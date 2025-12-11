export function scrollToBottom(element: HTMLElement) {
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
}

// expose globally
(window as any).blazorScrollToBottom = scrollToBottom;
