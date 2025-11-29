(function(){
    function ensureEditorInterop() {
        if (window.editorInterop) return Promise.resolve(true);
        // try to wait until editorInterop is available
        return new Promise((resolve) => {
            const start = Date.now();
            const check = () => {
                if (window.editorInterop) return resolve(true);
                if (Date.now() - start > 5000) return resolve(false);
                setTimeout(check, 50);
            };
            check();
        });
    }

    window.QuillFunctions = window.QuillFunctions || {};

    window.QuillFunctions.createQuill = async function (elementId, dotNetRef, options) {
        try {
            const ok = await ensureEditorInterop();
            if (!ok) return false;
            // delegate to editorInterop.initializeEditor
            return await window.editorInterop.initializeEditor(elementId, '', options);
        } catch (e) {
            console.error('QuillFunctions.createQuill error', e);
            return false;
        }
    };

    window.QuillFunctions.getHTML = function (elementId) {
        try {
            if (window.editorInterop && window.editorInterop.getEditorContent) return window.editorInterop.getEditorContent(elementId);
        } catch (e) { console.warn(e); }
        return '';
    };

    window.QuillFunctions.setHTML = function (elementId, html) {
        try {
            if (window.editorInterop && window.editorInterop.setEditorContent) return window.editorInterop.setEditorContent(elementId, html);
        } catch (e) { console.warn(e); }
    };

    window.QuillFunctions.destroy = function (elementId) {
        try {
            if (window.editorInterop && window.editorInterop.destroyEditor) return window.editorInterop.destroyEditor(elementId);
        } catch (e) { console.warn(e); }
    };

})();