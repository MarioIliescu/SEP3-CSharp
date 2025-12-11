window.blurActiveElement = function () {
    if (document.activeElement instanceof HTMLElement) {
        document.activeElement.blur();
    }
};