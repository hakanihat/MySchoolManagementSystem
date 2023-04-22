$(document).ready(function () {

    // Add short answer field on button click
    $("#add-textarea-answer-btn").on("click", function () {

        // Add new short answer field
        var newAnswer = $('<div class="form-group">' +
            '<label for="short-answer">Short Answer</label>' +
            '<div class="input-group">' +
            '<textarea id="short-answer" name="short-answer" class="form-control" style="width: 70%;"></textarea>' +
            '<button type="button" class="btn btn-danger delete-answer-btn" style="margin-left: 10px;"><i class="fas fa-trash"></i>Delete</button>' +
            '</div>' +
            '</div>');

        $("#answers-container").append(newAnswer);
    });

    // Add upload button for files
    $("#add-upload-file-btn").on("click", function () {
        var newUpload = $('<div class="form-group">' +
            '<label for="file-upload">Upload File</label>' +
            '<div class="input-group">' +
            '<input type="file" id="file-upload" name="file-upload" />' +
            '<button type="button" class="btn btn-danger delete-answer-btn" style="margin-left: 10px;"><i class="fas fa-trash"></i>Delete</button>' +
            '</div>' +
            '</div>');

        $("#answers-container").append(newUpload);
    });

    // Delete answer field on button click
    $("#answers-container").on("click", ".delete-answer-btn", function () {
        $(this).closest(".form-group").remove();
    });

});
