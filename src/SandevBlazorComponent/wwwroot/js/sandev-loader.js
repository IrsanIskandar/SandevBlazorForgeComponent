window.sandevLoaded = window.sandevLoaded || {};

window.sandevLoadCss = function (url) {
    if (window.sandevLoaded[url]) return;

    if (!document.querySelector(`link[href="${url}"]`)) {
        let link = document.createElement("link");
        link.rel = "stylesheet";
        link.href = url;
        document.head.appendChild(link);
    }

    window.sandevLoaded[url] = true;
};

window.sandevLoadScript = function (url) {
    if (window.sandevLoaded[url]) return;

    if (!document.querySelector(`script[src="${url}"]`)) {
        let script = document.createElement("script");
        script.src = url;
        script.defer = true;
        document.body.appendChild(script);
    }

    window.sandevLoaded[url] = true;
};

window.sandevSetTheme = function (isDark) {
    const theme = isDark ? "dark" : "light";
    document.documentElement.setAttribute("data-theme", theme);
};

window.sandevSetPrimaryColor = function (color) {
    document.documentElement.style.setProperty('--sandev-primary', color);
};

window.sandevGetSystemTheme = function () {
    return window.matchMedia('(prefers-color-scheme: dark)').matches;
};

window.sandevSaveTheme = function (isDark) {
    localStorage.setItem("sandev-theme", isDark ? "dark" : "light");
};

window.sandevLoadTheme = function () {
    return localStorage.getItem("sandev-theme");
};

window.sandevWatchSystemTheme = function (dotnetRef) {
    const media = window.matchMedia('(prefers-color-scheme: dark)');

    media.addEventListener('change', e => {
        dotnetRef.invokeMethodAsync('OnSystemThemeChanged', e.matches);
    });
};

