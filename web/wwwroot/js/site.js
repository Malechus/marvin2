// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
    var nav = document.getElementById("marvinNav");
    var toggle = document.getElementById("marvinNavToggle");
    if (!nav || !toggle) {
        return;
    }

    var storageKey = "marvinNavCollapsed";

    function setCollapsed(collapsed) {
        nav.classList.toggle("collapsed", collapsed);
        toggle.classList.toggle("collapsed", collapsed);
        toggle.setAttribute("aria-expanded", String(!collapsed));
        localStorage.setItem(storageKey, collapsed ? "1" : "0");
    }

    setCollapsed(localStorage.getItem(storageKey) === "1");

    toggle.addEventListener("click", function () {
        setCollapsed(!nav.classList.contains("collapsed"));
    });
})();
