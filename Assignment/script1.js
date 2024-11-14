document.addEventListener("DOMContentLoaded", function () {
    const imgSrc = localStorage.getItem("bioSrc");
    const bioName = localStorage.getItem("bioName");

    if (imgSrc && bioName) {
        document.getElementById("bioSrc").src = imgSrc;
        const bioNameElements = document.querySelectorAll(".bioName");
        bioNameElements.forEach(function(span) {
            span.innerHTML = bioName;
        });
    }

    let modifiedAltText = bioName;
    if (modifiedAltText) {
        modifiedAltText += " ";

        document.querySelectorAll(".bioName").forEach(function(span) {
            span.innerHTML = modifiedAltText;
        });
    }

    localStorage.removeItem("bioSrc");
    localStorage.removeItem("bioName");
});
