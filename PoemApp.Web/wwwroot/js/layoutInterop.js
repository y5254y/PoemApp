window.layoutInterop = (function () {
    const handlers = new Map();
    return {
        registerResizeHandler: function (dotNetRef, id, threshold) {
            if (!dotNetRef) return;
            const handler = function () {
                try {
                    const w = window.innerWidth || document.documentElement.clientWidth;
                    dotNetRef.invokeMethodAsync('OnBrowserResize', w);
                } catch (e) {
                    // ignore
                }
            };
            // call once immediately
            handler();
            window.addEventListener('resize', handler);
            handlers.set(id, handler);
        },
        unregisterResizeHandler: function (id) {
            const handler = handlers.get(id);
            if (handler) {
                window.removeEventListener('resize', handler);
                handlers.delete(id);
            }
        }
    };
})();
