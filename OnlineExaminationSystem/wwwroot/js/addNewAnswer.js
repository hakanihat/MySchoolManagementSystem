$(document).ready(function () {
    var answers = {}; // create empty dictionary to store answers

    $("#add-answer-btn").click(function () {
        var answerType = $(".answer-input").prop("type");
        var inputType = answerType === "radio" ? "radio" : "checkbox";
        var newInput = $("<div class='form-check'><input type='" + inputType + "' name='Answer' class='form-check-input answer-input' id='answer-" + ($(".answer-input").length + 1) + "' value='true'><label class='form-check-label' for='answer-" + ($(".answer-input").length + 1) + "'><input type='text' name='AnswerText" + ($(".answer-text").length + 1) + "' class='form-control answer-text' id='answer-text-" + ($(".answer-text").length + 1) + "' required /></label><button class='btn btn-danger delete-answer' type='button'>Delete</button></div>");
        $("#answers-container").append(newInput);
    });

    $("#answers-container").on("click", ".delete-answer", function () {
        $(this).parent().remove();
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

        answerInputs.each(function (index) {
            $(this).attr("value", "Answer" + (index + 1));
        });

        answerTexts.each(function (index) {
            $(this).attr("name", "AnswerText" + (index + 1));
        });
    });

  
});
