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

    // Store values in localStorage
    localStorage.setItem("bioSrc", imgSrc);
    localStorage.setItem("bioName", altText);

    // Redirect to the biography page
    window.location.href = "biography.html";
}


