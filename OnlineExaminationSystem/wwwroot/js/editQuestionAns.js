$(function () {
    $(document).on('submit', 'form', function () {
        var answers = [];

        // Loop through each answer input and create an object
        $('input[name="answer.AnswerText"]').each(function () {
            var answerText = $(this).val();
            var isCorrect = $(this).closest('.form-group').find('input[name="answer.IsCorrect"]').is(':checked');
            var answer = {
                AnswerText: answerText,
                IsCorrect: isCorrect
            };
            answers.push(answer);
        });

        // Serialize the answers array to JSON and set the value of the hidden input
        $('input[name="AnswersJson"]').val(JSON.stringify(answers));
    });
});