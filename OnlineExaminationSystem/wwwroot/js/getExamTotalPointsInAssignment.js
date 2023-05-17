$(document).ready(function () {
    // get initial max points
    var initialMaxPoints = parseFloat($('#MaxPoints').val());

    // update max points on exam selection change
    $('#examSelect').change(function () {
        var selectedExamId = $(this).val();
        if (selectedExamId) {
            $.getJSON('/Assignment/GetExamTotalPoints', { examId: selectedExamId }, function (data) {
                console.log('Success: ', data);
                var totalPoints = parseFloat(data);
                $('#MaxPoints').val(totalPoints);
            }).fail(function (error) {
                console.log('Error: ', error);
                // Restore initial max points in case of error
                $('#MaxPoints').val(initialMaxPoints);
            });
        } else {
            // Restore initial max points when no exam is selected
            $('#MaxPoints').val(initialMaxPoints);
        }
    });
});