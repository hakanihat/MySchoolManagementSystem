$(document).ready(function () {
    // Add click event listener to all delete buttons, even those created dynamically
    $(document).on('click', '.delete-question', function () {
        // Get the input element that corresponds to the clicked delete button
        var inputElement = $(this).prev('input');
        // Remove the input element from the DOM
        inputElement.remove();
        // Remove the parent form-group element
        $(this).parent().remove();
    });
});