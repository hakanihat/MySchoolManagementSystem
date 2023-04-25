$(function () {
    var $container = $('#create-exam-form'); // the container element where text inputs will be added

    // get the first text input and add it to the array
    var $firstQuestionInput = $('<input>').attr('type', 'text').addClass('form-control').prop('readonly', true);
    var $deleteButton = $('<button>').addClass('btn btn-danger').text('Delete');
    var $inputContainer = $('<div>').addClass('form-group input-container').append($firstQuestionInput).append($deleteButton);

    var questionInputs = [$inputContainer]; // an array to store all the text inputs
    var $questionDropdown = $('select[name="SelectedQuestionIds"]');
    $questionDropdown.find('option:first').prop('disabled', true);
    // event handler for when a dropdown value is selected
    function onQuestionSelected($dropdown) {
        var selectedQuestionText = $dropdown.find('option:selected').text();
        var selectedQuestionId = $dropdown.val();

        // check if "Select question" option was selected
        if (selectedQuestionText === 'Select question') {
            $dropdown.siblings('.text-danger').remove();
            $dropdown.after('<span class="text-danger">Please select a valid question.</span>');
            return;
        }
        // check if a question is already selected in another input
        var alreadySelected = false;
        for (var i = 0; i < questionInputs.length; i++) {
            if (questionInputs[i].data('question-id') === selectedQuestionId) {
                if (questionInputs[i].index() !== $dropdown.closest('.form-group').index()) {
                    alreadySelected = true;
                    break;
                }
            }
        }

        if (alreadySelected) {
            // if the question is already selected in another input, show an error message
            $dropdown.siblings('.text-danger').remove();
            $dropdown.after('<span class="text-danger">This question has already been selected in another input.</span>');
        } else {
            // if the question is not already selected in another input, remove any existing error message
            $dropdown.siblings('.text-danger').remove();

            var index = $dropdown.data('index');

            // remove all the text inputs after the one immediately after the selected one
            for (var i = questionInputs.length - 1; i > index + 1; i--) {
                questionInputs[i].remove();
                questionInputs.splice(i, 1);
            }

            // create a new text input for the selected question
            var $nextQuestionInput = $('<input>').attr('type', 'text').addClass('form-control').prop('readonly', true).val(selectedQuestionText);
            var $nextDeleteButton = $('<button>').addClass('btn btn-danger').text('Delete');
            var $nextInputContainer = $('<div>').addClass('form-group input-container').append($nextQuestionInput).append($nextDeleteButton);
            $nextInputContainer.data('question-id', selectedQuestionId);

            $nextDeleteButton.click(function () {
                $nextInputContainer.remove();
                questionInputs.splice(questionInputs.indexOf($nextInputContainer), 1);
            });

            // insert the new text input before the create button
            $container.find('#create-exam-inputs').before($nextInputContainer);
            questionInputs.splice(index + 1, questionInputs.length - index - 1, $nextInputContainer);




            // update the data-index attribute of the remaining dropdowns
            $container.find('select[name="SelectedQuestionIds"]').each(function (i, el) {
                $(el).data('index', i);
            });

        }
    }

    // attach the event handler to all the dropdowns
    $container.on('change', 'select[name="SelectedQuestionIds"]', function () {
        var $dropdown = $(this);
        onQuestionSelected($dropdown);
    });
});
