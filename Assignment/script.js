function myName() {
    let name = document.getElementById("search").value;
    let nameSpans = document.getElementsByClassName("name");

    for (let span of nameSpans) {
        span.innerHTML = name;
    }
}

function biography(imgElement) {
    const imgSrc = imgElement.src;
    const altText = imgElement.alt;

    localStorage.setItem("bioSrc", imgSrc);
    localStorage.setItem("bioName", altText);

    window.location.href = "biography.html";
}


