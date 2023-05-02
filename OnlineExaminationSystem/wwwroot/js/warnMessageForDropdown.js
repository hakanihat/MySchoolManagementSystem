$(document).ready(function () {
    var select = $('#SelectedQuestionIds');
    var container = $('#selectedQuestionsContainer');

    // Listen for changes in the select element
    select.on('change', function () {
        // Get the selected option
        var selectedOption = select.find(':selected');

        // Check if the selected option is the disabled one
        if (selectedOption.is(':disabled')) {
            $('#warning-message').show();
            return;
        }

        $('#warning-message').hide();

        // Rest of the code for adding the selected question to the container goes here...
    });
});