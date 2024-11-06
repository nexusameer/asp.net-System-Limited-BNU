document.addEventListener("DOMContentLoaded", function () {
    // Retrieve stored values from localStorage
    const imgSrc = localStorage.getItem("bioSrc");
    const bioName = localStorage.getItem("bioName");

    // Update the image source if data is available
    if (imgSrc && bioName) {
        document.getElementById("bioSrc").src = imgSrc;
        // Update all spans with the class 'bioName'
        const bioNameElements = document.querySelectorAll(".bioName");
        bioNameElements.forEach(function(span) {
            span.innerHTML = bioName;
        });
    }

    // Modify bioName text twice
    let modifiedAltText = bioName;
    if (modifiedAltText) {
        // Change 1: Append " - Player"
        modifiedAltText += " ";

        // Now update bioName with the final modified text
        document.querySelectorAll(".bioName").forEach(function(span) {
            span.innerHTML = modifiedAltText;
        });
    }

    // Clear the data from localStorage if needed
    localStorage.removeItem("bioSrc");
    localStorage.removeItem("bioName");
});
