export function registerClickOutside(dotNetObj, elementId) {
    function onClick(event) {
        const el = document.getElementById(elementId);
        if (el && !el.contains(event.target)) {
            dotNetObj.invokeMethodAsync('HideExtraColumn');
        }
    }

    document.addEventListener('click', onClick);

    return {
        dispose: function() {
            document.removeEventListener('click', onClick);
        }
    };
}

// optional: expose globally
window.blazorRegisterClickOutside = registerClickOutside;
