$(function () {
    var $groupSelect = $("select[name='Input.Group']");
    var $schoolNumberInput = $("input[name='Input.SchoolNumber']");
    var $schoolNumberLabel = $("label[for='Input_SchoolNumber']");
    $groupSelect.hide(); // hide the select by default
    $schoolNumberInput.hide(); // hide the SchoolNumber input by default
    $schoolNumberLabel.hide(); // hide the SchoolNumber label by default

    var $roleSelect = $("select[name='Input.Role']");
    $roleSelect.change(function () {
        if ($(this).val() === "student") {
            $groupSelect.show();
            $schoolNumberInput.show();
            $schoolNumberLabel.show();
        } else {
            $groupSelect.hide();
            $schoolNumberInput.hide();
            $schoolNumberLabel.hide();
        }
    });
});
