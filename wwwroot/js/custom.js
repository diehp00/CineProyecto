// custom.js

let lastScrollTop = 0;
const header = document.querySelector("header");

window.addEventListener("scroll", function () {
    let scrollTop = window.pageYOffset || document.documentElement.scrollTop;

    if (scrollTop > lastScrollTop) {
        // Desplazamiento hacia abajo
        header.classList.add("hide");
    } else {
        // Desplazamiento hacia arriba
        header.classList.remove("hide");
    }

    lastScrollTop = scrollTop;
});
