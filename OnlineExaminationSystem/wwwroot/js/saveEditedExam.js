$(document).ready(function () {
    // get initial list of questions
    var initialQuestions = [];
    $.getJSON('/Exam/GetQuestions', function (data) {
        initialQuestions = data;
        generateDropdown(initialQuestions);
        $('#CourseId').trigger('change'); // trigger the change event on page load

    });

    // filter questions based on selected course
    $('#CourseId').change(function () {
        var selectedCourseId = $(this).val();
        console.log('selectedCourseId:', selectedCourseId);
        if (selectedCourseId) {
            $.getJSON('/Exam/GetQuestions', { courseId: selectedCourseId }, function (data) {
                console.log('Success: ', data);
                var filteredQuestions = data;
                generateDropdown(filteredQuestions);

                // update selected question if applicable
                var selectedQuestion = $('#SelectedQuestionIds').val();
                if ($.inArray(selectedQuestion, $.map(filteredQuestions, function (question) { return question.value })) === -1) {
                    $('#SelectedQuestionIds').val('');
                }
            }).fail(function (error) {
                console.log('Error: ', error);
            });

        } else {
            generateDropdown(initialQuestions);
        }
    });





    function generateDropdown(questions) {
        // clear existing options
        $('#SelectedQuestionIds').empty();

        // add default option
        $('#SelectedQuestionIds').append($('<option>', {
            value: '',
            text: '-- Select a question --'
        }));

        // add questions as options
        $.each(questions, function (i, question) {
            $('#SelectedQuestionIds').append($('<option>', {
                value: question.value,
                text: question.text
            }));
        });
    }
});
