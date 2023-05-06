const form = document.querySelector('form');
const roleSelect = document.querySelector('select[name="Input.Role"]');
const groupSelect = document.querySelector('select[name="Input.Group"]');

form.addEventListener('submit', function (event) {
    if (roleSelect.value === 'student' && groupSelect.value === '') {
        console.log(roleSelect.value);

        toastr.warning('Please select a group');
        event.preventDefault();
    }
});

roleSelect.addEventListener('change', function () {
    if (roleSelect.value === 'student' && groupSelect.value === '') {
        console.log(roleSelect.value);

        groupSelect.setCustomValidity('Please select a group');
    } else {
        groupSelect.setCustomValidity('');
    }
});

groupSelect.addEventListener('change', function () {
    if (roleSelect.value === 'student' && groupSelect.value === '') {
        console.log(roleSelect.value);

        groupSelect.setCustomValidity('Please select a group');
    } else {
        groupSelect.setCustomValidity('');
    }
});
