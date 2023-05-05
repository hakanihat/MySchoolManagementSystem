const form = document.querySelector('form');
const groupSelect = document.querySelector('select[name="Input.Group"]');

form.addEventListener('submit', function (event) {
    if (groupSelect.value === '') {
        toastr.warning('Please select a group');
        event.preventDefault();
    }
});

groupSelect.addEventListener('change', function () {
    if (groupSelect.value === '') {
        groupSelect.setCustomValidity('Please select a group');
    } else {
        groupSelect.setCustomValidity('');
    }
});