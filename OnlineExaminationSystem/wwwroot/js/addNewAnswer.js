$(document).ready(function () {
    var answers = {}; // create empty dictionary to store answers

    $("#add-answer-btn").click(function () {
        var answerType = $(".answer-input").prop("type");
        var inputType = answerType === "radio" ? "radio" : "checkbox";
        var newInput = $("<div class='form-check'><input type='" + inputType + "' name='Answer' class='form-check-input answer-input' value='Answer" + ($(".answer-input").length + 1) + "'><input type='text' name='AnswerText" + ($(".answer-text").length + 1) + "' class='form-control answer-text' required /><button class='btn btn-danger delete-answer' type='button'>Delete</button></div>");
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

    $("form").submit(function () {
        answers = {}; // clear the dictionary before adding new values

        // iterate through each answer input and add its value to the dictionary
        $(".answer-input:checked").each(function () {
            answers[$(this).val()] = true;
        });

        // iterate through each text input and add its value to the dictionary
        $(".answer-text").each(function () {
            var answerKey = $(this).prev(".answer-input").val();
            var answerText = $(this).val();
            answers[answerText] = answerKey;
        });

        // set the value of the hidden Answers input to the JSON representation of the dictionary
        $("#Answers").val(JSON.stringify(answers));
    });
});
