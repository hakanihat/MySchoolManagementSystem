$(function () {


    $(document).on('click', '.delete-answer', function () {
        $(this).closest('.form-group').remove();
    });


});