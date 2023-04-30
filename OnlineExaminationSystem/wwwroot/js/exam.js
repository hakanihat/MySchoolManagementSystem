$(document).ready(function () {
    $("#SelectedQuestionIds").change(function () {
        console.log("Hello");
        var selectedQuestions = $(this).val();
        var selectedQuestionsContainer = $("#selectedQuestionsContainer");
        selectedQuestionsContainer.empty();

        if (selectedQuestions.length > 0) {
            for (var i = 0; i < selectedQuestions.length; i++) {
                var questionInput = $("<input />").attr("type", "text").attr("name", "Question" + selectedQuestions[i]).attr("class", "form-control").attr("placeholder", "Answer for Question " + selectedQuestions[i]);
                selectedQuestionsContainer.append(questionInput);
            }
        }
    });
});


