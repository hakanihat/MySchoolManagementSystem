var $startDateInput = $('#start-time-input');
var $endDateInput = $('#end-time-input');

$startDateInput.on('change', validateDates);
$endDateInput.on('change', validateDates);

function validateDates() {
    var startDate = new Date($startDateInput.val());
    var endDate = new Date($endDateInput.val());

    // Check for invalid dates
    if (isNaN(startDate.getTime()) || isNaN(endDate.getTime())) {
        $endDateInput[0].setCustomValidity('Invalid date');
        return;
    }

    // Check for end date before start date
    if (endDate < startDate) {
        $endDateInput[0].setCustomValidity('End date must be after start date');
        return;
    }

    $endDateInput[0].setCustomValidity('');
}
