$(function () {
    var $groupSelect = $("select[name='Input.Group']");
    var $schoolNumberInput = $("input[name='Input.SchoolNumber']");
    $groupSelect.hide(); // hide the select by default
    $schoolNumberInput.hide(); // hide the SchoolNumber input by default

    var $roleSelect = $("select[name='Input.Role']");
    $roleSelect.change(function () {
        if ($(this).val() === "student") {
            $groupSelect.show();
            $schoolNumberInput.show();
        } else {
            $groupSelect.hide();
            $schoolNumberInput.hide();
        }
    });
});
