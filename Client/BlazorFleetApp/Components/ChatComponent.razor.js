export function scrollToBottom(element) {
    if (element) {
        // scroll to the very bottom of the scrollable content
        element.scrollTop = element.scrollHeight - element.clientHeight;
    }
}

// optional: expose globally
window.blazorScrollToBottom = scrollToBottom;
