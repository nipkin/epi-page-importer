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