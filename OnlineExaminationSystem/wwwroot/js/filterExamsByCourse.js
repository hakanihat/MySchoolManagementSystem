    $(document).ready(function () {
        // get initial list of exams
        var initialExams = [];
    $.getJSON('/Exam/GetExams', function (data) {
        initialExams = data;
    generateDropdown(initialExams);
        });

    // filter exams based on selected course
    $('#courseSelect').change(function () {
            var selectedCourseId = $(this).val();
    console.log('selectedCourseId:', selectedCourseId);
    if (selectedCourseId) {
        $.getJSON('/Exam/GetExams', { courseId: selectedCourseId }, function (data) {
            console.log('Success: ', data);
            var filteredExams = data;
            generateDropdown(filteredExams);
        }).fail(function (error) {
            console.log('Error: ', error);
        });

            } else {
        generateDropdown(initialExams);
            }
        });

    function generateDropdown(exams) {
        // clear existing options
        $('#examSelect').empty();

    // add default option
    $('#examSelect').append($('<option>', {
        value: '',
        text: '-- Select an exam --'
            }));

        // add exams as options
        $.each(exams, function (i, exam) {
            $('#examSelect').append($('<option>', {
                value: exam.value,
                text: exam.text
            }));
            });
        }
    });

