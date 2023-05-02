$(document).ready(function () {
    var select = $('#SelectedQuestionIds');
    var container = $('#selectedQuestionsContainer');
    var questionInputs = container.find('input[type="text"]');

    // Listen for changes in the select element
    select.on('change', function () {
        console.log('Select changed:', select.val());

        // Get the selected option
        var selectedOption = select.find(':selected');

        // Check if the option is not the default option and not already in the container
        if (selectedOption.val() && !container.find('#question' + selectedOption.val()).length) {
            console.log('Adding question:', selectedOption.val());

            // Create a new input and add it to the container
            var input = $('<input>').attr({
                type: 'text',
                class: 'form-control',
                name: 'Question' + selectedOption.val(),
                id: 'Question' + selectedOption.val(),
                value: selectedOption.text(),
                'data-question-id': selectedOption.val()
            });
            var label = $('<label>').attr('for', 'Question' + selectedOption.val()).text(selectedOption.text());
            var button = $('<button>').attr({
                type: 'button',
                class: 'btn btn-danger btn-sm delete-question',
            }).text('Delete');
            var div = $('<div>').attr({
                id: 'question' + selectedOption.val(),
                class: 'form-group'
            }).append(label).append(input).append(button);
            container.append(div);
        } else if (container.find('#question' + selectedOption.val()).length) {
            console.log('Question already selected:', selectedOption.val());

            // Show warning message using toastr if the option is already in the container
            toastr.warning('This question is already selected');
        }
        updateSelectedQuestionIds();
    });

    // Listen for clicks on the delete button
    container.on('click', '.delete-question', function () {
        console.log('Deleting question:', $(this).parent().attr('id'));

        // Remove the parent div when the delete button is clicked
        $(this).parent().remove();
        updateSelectedQuestionIds();
    });

    function updateSelectedQuestionIds() {
        var selectedQuestionIds = [];
        $('#selectedQuestionsContainer').find('.form-group').each(function () {
            var questionId = $(this).attr('id').replace('question', '');
            selectedQuestionIds.push(questionId);
        });
        console.log('Selected question ids:', selectedQuestionIds);
        $('#SelectedQuestionIdsJson').val(JSON.stringify(selectedQuestionIds));
    }

});
