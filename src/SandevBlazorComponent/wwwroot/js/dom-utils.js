let clickOutsideHandlers = new Map();

export function registerClickOutside(element, dotnetRef) {
    function handler(event) {
        if (!element.contains(event.target)) {
            dotnetRef.invokeMethodAsync("OnClickOutside");
        }
    }

    document.addEventListener("mousedown", handler);
    clickOutsideHandlers.set(element, handler);
}

export function unregisterClickOutside(element) {
    const handler = clickOutsideHandlers.get(element);
    if (handler) {
        document.removeEventListener("mousedown", handler);
        clickOutsideHandlers.delete(element);
    }
}

export function focusElement(element) {
    element?.focus();
}

export function positionPopup(target, popup) {
    if (!target || !popup) return;

    const rect = target.getBoundingClientRect();
    const popupRect = popup.getBoundingClientRect();

    const vw = window.innerWidth;
    const vh = window.innerHeight;

    let top = rect.bottom;
    let left = rect.left;

    // flip ke atas kalau mentok bawah
    if (rect.bottom + popupRect.height > vh) {
        top = rect.top - popupRect.height;
    }

    // geser kalau keluar kanan
    if (rect.left + popupRect.width > vw) {
        left = vw - popupRect.width - 10;
    }

    if (left < 10) left = 10;

    popup.style.position = "fixed";
    popup.style.top = `${top}px`;
    popup.style.left = `${left}px`;
    popup.style.zIndex = "9999";
}