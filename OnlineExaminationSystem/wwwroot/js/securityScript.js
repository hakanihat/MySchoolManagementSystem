// Disable right-click
document.addEventListener("contextmenu", function (e) {
    e.preventDefault();
});

// Disable copying and cutting key combinations
document.addEventListener("keydown", function (e) {
    if (e.ctrlKey && (e.key === "c" || e.key === "x" || e.key === "f")) {
        e.preventDefault();
    }
});

// Prevent manipulating the URL and clicking on links
document.addEventListener("DOMContentLoaded", function () {
    var links = document.getElementsByTagName("a");
    for (var i = 0; i < links.length; i++) {
        links[i].addEventListener("click", function (e) {
            e.preventDefault();
        });
    }
});

var isNewTabOpened = false;
var isFormSubmitted = false;
var numTabOpenAttempts = 0;
var maxTabOpenAttempts = 2;

// Detect when a new tab or window is opened
window.addEventListener('blur', function () {
    if (!isFormSubmitted) {
        if (!isNewTabOpened) {
            isNewTabOpened = true;
            toastr.warning("You are not allowed to open a new tab or window during the exam.", "Warning");
        } else {
            numTabOpenAttempts++;
            if (numTabOpenAttempts >= maxTabOpenAttempts) {
                submitFormAsPunishment();
            }
        }
    }
});

// Submit the form as a punishment
function submitFormAsPunishment() {
    if (!isFormSubmitted) {
        isFormSubmitted = true;
        toastr.error("You have violated the exam rules by opening a new tab. Your exam will be submitted as a punishment.", "Violation");
        document.getElementById('examForm').submit();
    }
}