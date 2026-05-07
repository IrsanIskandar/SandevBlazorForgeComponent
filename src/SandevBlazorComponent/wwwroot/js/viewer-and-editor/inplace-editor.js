let clickOutsideMap = new Map();
let popupMap = new Map(); // 🔥 untuk tracking popup

// ===============================
// CLICK OUTSIDE
// ===============================
export function registerClickOutside(element, dotnetRef) {
    if (!element) return;

    function handler(e) {
        if (!element.contains(e.target)) {
            dotnetRef.invokeMethodAsync("OnClickOutside");
        }
    }

    document.addEventListener("pointerdown", handler);
    clickOutsideMap.set(element, handler);
}

export function unregisterClickOutside(element) {
    const handler = clickOutsideMap.get(element);
    if (handler) {
        document.removeEventListener("pointerdown", handler);
        clickOutsideMap.delete(element);
    }
}

// ===============================
// FOCUS
// ===============================
export function focusElement(element) {
    if (element && typeof element.focus === "function") {
        element.focus();
    }
}

export function focusFirstInput(container) {
    if (!container) return;

    const el = container.querySelector("input, textarea, select, [contenteditable='true']");
    if (el && typeof el.focus === "function") {
        el.focus();
    }
}

// ===============================
// POPUP POSITION
// ===============================
function calculatePosition(target, popup) {
    const rect = target.getBoundingClientRect();
    const popupRect = popup.getBoundingClientRect();

    const viewportWidth = window.innerWidth;
    const viewportHeight = window.innerHeight;

    let top = rect.bottom;
    let left = rect.left;

    // overflow bawah → pindah ke atas
    if (rect.bottom + popupRect.height > viewportHeight) {
        top = rect.top - popupRect.height;
    }

    // overflow kanan → geser kiri
    if (rect.left + popupRect.width > viewportWidth) {
        left = viewportWidth - popupRect.width - 10;
    }

    if (left < 10) left = 10;

    popup.style.position = "fixed";
    popup.style.top = `${top}px`;
    popup.style.left = `${left}px`;
    popup.style.zIndex = "9999";
}

// 🔥 register popup + auto update
export function positionPopup(target, popup) {
    if (!target || !popup) return;

    function update() {
        calculatePosition(target, popup);
    }

    update();

    // simpan supaya bisa di-unregister nanti
    popupMap.set(popup, update);

    window.addEventListener("scroll", update);
    window.addEventListener("resize", update);
}

// 🔥 WAJIB untuk cleanup
export function unregisterPopup(popup) {
    const update = popupMap.get(popup);
    if (update) {
        window.removeEventListener("scroll", update);
        window.removeEventListener("resize", update);
        popupMap.delete(popup);
    }
}
