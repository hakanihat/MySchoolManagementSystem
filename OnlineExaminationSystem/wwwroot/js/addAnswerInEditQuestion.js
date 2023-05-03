$(function () {
    $('#add-answer').click(function () {
        var index = $('#answers').children().length;
        var answerHtml = '<div class="form-group">' +
            '<label>Answer</label>' +
            '<input type="text" name="answer.AnswerText" value="" class="form-control" />' +
            '<span class="text-danger field-validation-valid" data-valmsg-for="answer.AnswerText" data-valmsg-replace="true"></span>' +
            '<div class="form-check">' +
            '<input class="form-check-input" type="checkbox" name="answer.IsCorrect" value="" >' +
            '<label class="form-check-label">Is Correct</label>' +
            '</div>' +
            '<button type="button" class="btn btn-danger delete-answer">Delete</button>' +
            '</div>';
        $('#answers').append(answerHtml);
    });

    $(document).on('click', '.delete-answer', function () {
        $(this).closest('.form-group').remove();
    });
});