let handlers = {};

export function registerClickOutside(element, dotnetRef) {
    function handler(event) {
        if (!element.contains(event.target)) {
            dotnetRef.invokeMethodAsync("OnClickOutside");
        }
    }

    document.addEventListener("mousedown", handler);
    handlers[element] = handler;
}

export function unregisterClickOutside(element) {
    const handler = handlers[element];
    if (handler) {
        document.removeEventListener("mousedown", handler);
        delete handlers[element];
    }
}

export function focusElement(element) {
    if (element) {
        element.focus();
    }
}

export function positionPopup(target, popup) {
    if (!target || !popup) return;

    const rect = target.getBoundingClientRect();
    const popupRect = popup.getBoundingClientRect();

    const viewportWidth = window.innerWidth;
    const viewportHeight = window.innerHeight;

    let top = rect.bottom;
    let left = rect.left;

    // 🔥 cek overflow bawah → pindah ke atas
    if (rect.bottom + popupRect.height > viewportHeight) {
        top = rect.top - popupRect.height;
    }

    // 🔥 cek overflow kanan → geser kiri
    if (rect.left + popupRect.width > viewportWidth) {
        left = viewportWidth - popupRect.width - 10;
    }

    // 🔥 minimal padding kiri
    if (left < 10) left = 10;

    popup.style.position = "fixed";
    popup.style.top = `${top}px`;
    popup.style.left = `${left}px`;
    popup.style.zIndex = "9999";
}

window.addEventListener("scroll", () => {
    positionPopup(target, popup);
});

window.addEventListener("resize", () => {
    positionPopup(target, popup);
});
