window.sandevCaptcha = {
    init: function () {
        let points = [];

        const addPoint = (e) => {
            points.push({
                x: e.clientX,
                y: e.clientY,
                time: new Date().toISOString()
            });

            if (points.length > 20)
                points.shift(); // limit data
        };

        document.addEventListener('mousemove', addPoint);
        document.addEventListener('click', addPoint);

        const fingerprint = navigator.userAgent + "|" + screen.width + "x" + screen.height;

        return {
            getData: () => ({
                fingerprint: fingerprint,
                points: points
            })
        };
    }
};