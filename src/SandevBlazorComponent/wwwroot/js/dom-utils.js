let clickOutsideMap = new Map();

//export function registerClickOutside(element, dotnetRef) {
//    function handler(event) {
//        setTimeout(() => {
//            if (!element.contains(event.target)) {
//                dotnetRef.invokeMethodAsync("OnClickOutside");
//            }
//        }, 0);
//    }

//    document.addEventListener("click", handler);
//    clickOutsideMap.set(element, handler);
//}

//export function unregisterClickOutside(element) {
//    const handler = clickOutsideMap.get(element);
//    if (handler) {
//        document.removeEventListener("click", handler);
//        clickOutsideMap.delete(element);
//    }
//}

export function registerClickOutside(element, dotnetRef) {
    if (!element) return;

    function handler(e) {
        if (!element.contains(e.target)) {
            dotnetRef.invokeMethodAsync("OnClickOutside");
        }
    }

    document.addEventListener("mousedown", handler);
    clickOutsideMap.set(element, handler);
}

export function unregisterClickOutside(element) {
    const handler = clickOutsideMap.get(element);
    if (handler) {
        document.removeEventListener("mousedown", handler);
        clickOutsideMap.delete(element);
    }
}

export function focusElement(element) {
    if (element && typeof element.focus === "function") {
        element.focus();
    }
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

export function focusFirstInput(container) {
    if (!container) return;

    const el = container.querySelector("input, textarea, select, [contenteditable='true']");
    if (el && typeof el.focus === "function") {
        el.focus();
    }
}

