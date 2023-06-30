window.addEventListener("DOMContentLoaded", function () {
    var examId = @Html.Raw(Json.Serialize(Model.ExamId)); // get examId from the model

    if (examId) { // check if examId is not null
        var examSelect = document.getElementById("examSelect");

        for (var i = 0; i < examSelect.options.length; i++) {
            if (examSelect.options[i].value == examId) {
                examSelect.selectedIndex = i;
                break;
            }
        }
    }
});