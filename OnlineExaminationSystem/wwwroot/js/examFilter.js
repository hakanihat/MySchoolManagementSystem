$(document).ready(function () {
    // Function to filter exams based on the provided filter
    function filterExams() {
        var filterValue = $('#filterInput').val().trim().toLowerCase();

        $('.exam-row').each(function () {
            var examName = $(this).data('exam-name').toLowerCase();
            var course = $(this).data('course').toLowerCase();

            if (examName.includes(filterValue) || course.includes(filterValue)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    }

    // Event handler for the filter input
    $('#filterInput').on('input', function () {
        filterExams();
    });
});