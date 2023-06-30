document.getElementById("searchBar").addEventListener("input", function () {
    var input = this.value.toLowerCase();
    var rows = document.querySelectorAll("tbody tr");
    for (var i = 0; i < rows.length; i++) {
        var schoolNumber = rows[i].querySelectorAll("td")[0].textContent.toLowerCase();
        var fullName = rows[i].querySelectorAll("td")[1].textContent.toLowerCase();
        var group = rows[i].querySelectorAll("td")[2].textContent.toLowerCase();
        if (schoolNumber.indexOf(input) > -1 || fullName.indexOf(input) > -1 || group.indexOf(input) > -1) {
            rows[i].style.display = "";
        } else {
            rows[i].style.display = "none";
        }
    }
});