const input = document.getElementById("recipe-search");
const results = document.getElementById("recipe-results");

let timeout = null;

input.addEventListener("input", function () {
    clearTimeout(timeout);

    timeout = setTimeout(() => {
        const q = input.value;
        if (q.length < 2) {
            results.innerHTML = "";
            return;
        }

        fetch(`/api/recipe-search?q=${encodeURIComponent(q)}`)
            .then(r => r.json())
            .then(data => {
                console.log(data);
                results.innerHTML = data.map(x =>
                    `<li><a href="${x.url}">${x.title}</a></li>`
                ).join("");
            });
    }, 300);
});


const burger = document.querySelector(".hamburger");
const menu = document.querySelector(".main-menu");

burger.addEventListener("click", () => {
    burger.classList.toggle("active");
    menu.classList.toggle("show");
});

// Mobile submenu toggle
menu.querySelectorAll("li > a").forEach(link => {
    link.addEventListener("click", e => {
        const parent = link.parentElement;
        if (parent.querySelector("ul") && window.innerWidth < 900) {
            e.preventDefault();
            parent.classList.toggle("open");
        }
    });
});


function myFunction() {
    var x = document.getElementById("myTopnav");
    if (x.className === "main-menu") {
        x.className += " responsive";
    } else {
        x.className = "main-menu";
    }
}