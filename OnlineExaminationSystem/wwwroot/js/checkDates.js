$(document).ready(function () {
    // Initialize the datepicker for the start and end time inputs
    flatpickr("#start-time-input", {
        enableTime: true,
        dateFormat: "Y-m-d H:i",
    });
    flatpickr("#end-time-input", {
        enableTime: true,
        dateFormat: "Y-m-d H:i",
    });

    // Get the form element
    var form = $("#create-exam-form");

    // Create a new error message element
    var errorMessage = $("<span>")
        .attr("id", "time-error")
        .css("color", "red")
        .hide();

    // Add the error message element to the page
    $("#end-time-input").after(errorMessage);

    // Attach a submit event listener to the form
    form.submit(function (event) {
        // Get the start and end time values
        var startTime = $("#start-time-input").val();
        var endTime = $("#end-time-input").val();

        // Check if the start time is earlier than the end time
        if (startTime >= endTime) {
            // Prevent the form from submitting
            event.preventDefault();

            // Update the error message text and show it
            errorMessage.text("The start time must be earlier than the end time.");
            errorMessage.show();
        } else {
            // Hide the error message if it's currently shown
            errorMessage.hide();
        }
    });
});
