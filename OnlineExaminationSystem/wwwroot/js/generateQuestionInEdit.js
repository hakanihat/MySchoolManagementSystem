$(function () {
    $('#SelectedQuestionIds').change(function () {
        var selectedQuestion = $(this).find(":selected");
        var questionName = selectedQuestion.text();
        var questionId = selectedQuestion.val();
        var questionInput = '<input type="text" name="Question" value="' + questionName + '" />';
        $('#generatedQuestions').empty().append(questionInput);
    });
});
