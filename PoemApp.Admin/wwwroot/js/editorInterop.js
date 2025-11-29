window.editorInterop = (function () {
    const instances = {};
    const createdPlaceholders = new Set();

    function addScript(url) {
        return new Promise((resolve, reject) => {
            if (document.querySelector(`script[src="${url}"]`)) return resolve();
            const s = document.createElement('script');
            s.src = url;
            s.async = false;
            s.onload = () => { console.log('Loaded script', url); resolve(); };
            s.onerror = () => { console.error('Failed to load script', url); reject(new Error('Failed to load ' + url)); };
            document.head.appendChild(s);
        });
    }

    function addCss(url) {
        return new Promise((resolve, reject) => {
            if (document.querySelector(`link[href="${url}"]`)) return resolve();
            const l = document.createElement('link');
            l.href = url;
            l.rel = 'stylesheet';
            l.onload = () => { console.log('Loaded css', url); resolve(); };
            l.onerror = () => { console.error('Failed to load css', url); reject(new Error('Failed to load css ' + url)); };
            document.head.appendChild(l);
        });
    }

    function waitForQuill(timeoutMs = 5000) {
        return new Promise((resolve, reject) => {
            const start = Date.now();
            const check = () => {
                if (window.Quill) return resolve(true);
                if (Date.now() - start > timeoutMs) return reject(new Error('Quill did not become available'));
                setTimeout(check, 50);
            };
            check();
        });
    }

    function waitForElementByObserver(id, timeoutMs = 5000) {
        // if caller passed an actual element, resolve immediately
        if (id && typeof id === 'object' && id.nodeType === 1) return Promise.resolve(id);

        return new Promise((resolve, reject) => {
            const existing = (typeof id === 'string' && id) ? document.getElementById(id) : null;
            if (existing) return resolve(existing);

            const observer = new MutationObserver((mutations) => {
                for (const m of mutations) {
                    for (const node of m.addedNodes) {
                        if (node.nodeType === 1) {
                            let el = null;
                            if (typeof id === 'string' && id) {
                                try {
                                    el = node.querySelector ? node.querySelector('#' + id) : null;
                                } catch (e) {
                                    // ignore invalid selector errors
                                    el = null;
                                }
                                if (el) { observer.disconnect(); return resolve(el); }
                            }
                            if (node.id && typeof id === 'string' && node.id === id) {
                                observer.disconnect(); return resolve(node);
                            }
                        }
                    }
                }
            });

            observer.observe(document.body, { childList: true, subtree: true });

            const timeout = setTimeout(() => {
                observer.disconnect();
                reject(new Error('Element not found (observer): ' + id));
            }, timeoutMs);
        });
    }

    async function loadQuill() {
        if (window.Quill) return true;
        const localScript = '/lib/quill/quill.min.js';
        const localCss = '/lib/quill/quill.snow.css';
        const cdnScript = 'https://cdn.quilljs.com/1.3.6/quill.min.js';
        const cdnCss = 'https://cdn.quilljs.com/1.3.6/quill.snow.css';

        // try local first
        try {
            await addCss(localCss);
            await addScript(localScript);
            await waitForQuill();
            console.log('Quill loaded from local');
            return true;
        } catch (e) {
            console.warn('editorInterop.loadQuill: local load failed, falling back to CDN', e);
        }

        // fallback to CDN
        try {
            await addCss(cdnCss);
            await addScript(cdnScript);
            await waitForQuill();
            console.log('Quill loaded from CDN');
            return true;
        } catch (e) {
            console.error('editorInterop.loadQuill: CDN load failed', e);
            return false;
        }
    }

    function findBestParentForEditor() {
        // Prefer the immediate MudPaper container if present
        const mudPaper = document.querySelector('.mud-paper');
        if (mudPaper) return mudPaper;
        // Fallback to main content container
        const main = document.querySelector('.mud-main-content') || document.querySelector('.mud-container');
        if (main) return main;
        // Fallback to form parent
        const form = document.querySelector('form');
        if (form && form.parentElement) return form.parentElement;
        // Last resort: body
        return document.body;
    }

    async function initializeEditor(id, htmlContent, options, dotNetRef) {
        // accept either an element or an id string
        // ensure Quill is loaded (if not yet, try to load it)
        if (!window.Quill) {
            try {
                const ok = await loadQuill();
                if (!ok) {
                    console.error('Quill is not loaded and could not be loaded');
                    return false;
                }
            } catch (e) {
                console.error('initializeEditor: failed to load Quill', e);
                return false;
            }
        }

        let container = null;
        let elementId = null;

        if (id && typeof id === 'object' && id.nodeType === 1) {
            container = id;
            elementId = id.id || null;
        } else if (typeof id === 'string' && id) {
            container = document.getElementById(id);
            elementId = id;
        }

        if (!container) {
            // wait for element by observing DOM, longer timeout
            try {
                const el = await waitForElementByObserver(id, 3000);
                container = el;
                elementId = el && el.id ? el.id : elementId;
            } catch (e) {
                console.warn('Editor container not found via observer within timeout', id);
            }
        }

        if (!container) {
            // create a container inserted into the most likely parent
            const parent = findBestParentForEditor();
            const newContainer = document.createElement('div');
            // determine an id to use
            const assignedId = (typeof id === 'string' && id) ? id : ('quill-' + Math.random().toString(36).slice(2, 11));
            newContainer.id = assignedId;
            newContainer.style.minHeight = '200px';
            newContainer.style.border = '1px dashed #ccc';
            parent.appendChild(newContainer);
            createdPlaceholders.add(assignedId);
            container = newContainer;
            elementId = assignedId;
            console.warn('Inserted editor container into parent:', parent.tagName, 'using id', assignedId);
        }

        // Create editor instance if not exists
        if (!instances[elementId]) {
            const toolbarOptions = (options && options.toolbar) || [
                [{ header: [1, 2, 3, false] }],
                ['bold', 'italic', 'underline', 'strike'],
                [{ list: 'ordered' }, { list: 'bullet' }],
                ['blockquote', 'code-block'],
                [{ indent: '-1' }, { indent: '+1' }],
                [{ align: [] }],
                ['link', 'image'],
                ['clean']
            ];

            const quill = new Quill(container, {
                theme: 'snow',
                modules: {
                    toolbar: toolbarOptions
                }
            });

            // attach custom image handler if image button exists
            const toolbar = quill.getModule('toolbar');
            if (toolbar && toolbar.container) {
                toolbar.addHandler('image', function() {
                    const input = document.createElement('input');
                    input.setAttribute('type', 'file');
                    input.setAttribute('accept', 'image/*');
                    input.onchange = () => {
                        const file = input.files[0];
                        if (!file) return;
                        const reader = new FileReader();
                        reader.onload = () => {
                            const base64 = reader.result;
                            const range = quill.getSelection(true);
                            quill.insertEmbed(range.index, 'image', base64);
                            quill.setSelection(range.index + 1);
                        };
                        reader.readAsDataURL(file);
                    };
                    input.click();
                });
            }

            instances[elementId] = quill;
            console.log('Quill editor instance created for', elementId);

            // If a DotNet reference is provided, wire up change events to call back
            if (dotNetRef && typeof dotNetRef.invokeMethodAsync === 'function') {
                quill.on('text-change', function() {
                    try {
                        const html = quill.root.innerHTML;
                        dotNetRef.invokeMethodAsync('NotifyContentChanged', html).catch(e => console.error('dotNet Invoke failed', e));
                    } catch (e) {
                        console.warn('Failed to invoke NotifyContentChanged:', e);
                    }
                });
            }
        }

        if (htmlContent) {
            instances[elementId].root.innerHTML = htmlContent;
        }

        return true;
    }

    function getEditorContent(id) {
        const inst = instances[id];
        if (!inst) return '';
        return inst.root.innerHTML;
    }

    function setEditorContent(id, htmlContent) {
        const inst = instances[id];
        if (!inst) return;
        inst.root.innerHTML = htmlContent || '';
    }

    function destroyEditor(id) {
        if (instances[id]) {
            const inst = instances[id];
            const container = document.getElementById(id);
            if (container) container.innerHTML = '';
            delete instances[id];
            console.log('Quill editor destroyed for', id);
            if (createdPlaceholders.has(id)) {
                // remove the placeholder element we inserted
                const el = document.getElementById(id);
                if (el && el.parentNode) el.parentNode.removeChild(el);
                createdPlaceholders.delete(id);
                console.log('Removed created placeholder for', id);
            }
        }
    }

    // expose both namespaced object and globals to avoid cases where Blazor can't find nested function; keep auto-load and diagnostics logs
    const api = {
        loadQuill: loadQuill,
        initializeEditor: initializeEditor,
        getEditorContent: getEditorContent,
        setEditorContent: setEditorContent,
        destroyEditor: destroyEditor
    };

    try {
        window.editorInterop = api;
        // also expose globals for direct lookup
        window.loadQuill = loadQuill;
        window.initializeEditor = initializeEditor;
        window.getEditorContent = getEditorContent;
        window.setEditorContent = setEditorContent;
        window.destroyEditor = destroyEditor;
    } catch (e) {
        console.warn('Failed to assign global editorInterop functions', e);
    }

    // Attempt auto-load to reduce chance of race when component immediately initializes
    (async function autoLoad() {
        try {
            await loadQuill();
        } catch (e) {
            // ignore
        }
    })();

    return api;
})();