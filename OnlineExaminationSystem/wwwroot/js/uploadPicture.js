$(document).ready(function () {
    var answerType = $(".answer-input").prop("type");

    $("#add-answer-btn").click(function () {
        var inputType = answerType === "radio" ? "radio" : "checkbox";
        var newInput = $("<div class='form-check'><input type='" + inputType + "' name='Answer' class='form-check-input answer-input' value='Answer" + ($(".answer-input").length + 1) + "'><input type='text' name='answerText' class='form-control answer-text' required /><button class='btn btn-danger delete-answer' type='button'>Delete</button></div>");
        $("#answers-container").append(newInput);
        updateAnswerInputs();
    });

    $("#answers-container").on("click", ".delete-answer", function () {
        $(this).parent().remove();
        updateAnswerInputs();
    });

    $("#toggle-answer-type").click(function () {
        var answerInputs = $(".answer-input");
        var answerTexts = $(".answer-text");
        var answerType = answerInputs.prop("type");
        var inputType = answerType === "radio" ? "checkbox" : "radio";

        answerInputs.prop("type", inputType);

        if (inputType === "radio") {
            $("#toggle-answer-type").text("Change to Checkbox");
        } else {
            $("#toggle-answer-type").text("Change to Radio");
        }

        updateAnswerInputs();
    });

    function updateAnswerInputs() {
        var answerInputs = $(".answer-input");
        var answerTexts = $(".answer-text");

        answerInputs.each(function (index) {
            var key = "Answer" + (index + 1);
            $(this).attr("value", key);
            $(this).attr("name", "Answers[" + key + "]");
        });

        answerTexts.each(function (index) {
            $(this).attr("name", "AnswerTexts[" + (index + 1) + "]");
        });
    }
});
