$(document).ready(function () {
    $("#form-id").submit(function () {
        console.log("Form submitted!");

        var answers = [];

        $(".answer-input").each(function () {
            var answerText = document.getElementById('answer-text-' + this.id.split('-')[1]).value;
    
            var isCorrect = $(this).is(":checked");
            console.log(isCorrect);
            var answer = { AnswerText: answerText, IsCorrect: isCorrect };
            console.log(answer);
            answers.push(answer);
            console.log(answers);

        });

        // Set the value of the hidden input field to a JSON representation of the answers array
        $('#AnswersJson').val(JSON.stringify(answers));

        // Allow the form to be submitted
        return true;
    });
});
