$(document).ready(function () {
    $('input[name^="questions"]').on('change', function () {
        var questionName = $(this).attr('name');
        var uncheckedCount = $('input[name="' + questionName + '"]:checked').not('[value="unanswered"]').length;
        if (uncheckedCount === 0) {
            $('input[name="' + questionName + '"][value="unanswered"]').prop('checked', true);
        } else {
            $('input[name="' + questionName + '"][value="unanswered"]').prop('checked', false);
        }
    });
});
