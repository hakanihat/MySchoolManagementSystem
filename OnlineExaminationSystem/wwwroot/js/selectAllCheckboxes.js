document.getElementById("selectAllCheckbox").addEventListener("click", function () {
    var checkboxes = document.querySelectorAll('input[type="checkbox"]');
    for (var i = 0; i < checkboxes.length; i++) {
        var row = checkboxes[i].parentNode.parentNode;
        if (checkboxes[i].id !== "selectAllCheckbox" && row.style.display !== "none") {
            checkboxes[i].checked = this.checked;
        }
    }
});