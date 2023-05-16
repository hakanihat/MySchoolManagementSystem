$(function () {
    var $groupSelect = $("select[name='Input.Group']");
    var $schoolNumberInput = $("input[name='Input.SchoolNumber']");
    var $schoolNumberLabel = $("label[for='Input_SchoolNumber']");
    var $coursesDiv = $("div.form-floating select[name='Input.Courses']").closest("div.form-floating");

    $groupSelect.hide();
    $schoolNumberInput.hide();
    $schoolNumberLabel.hide();
    $coursesDiv.hide();

    var $roleSelect = $("select[name='Input.Role']");
    $roleSelect.change(function () {
        if ($(this).val() === "student") {
            $groupSelect.show();
            $schoolNumberInput.show();
            $schoolNumberLabel.show();
            $coursesDiv.hide();
        } else if ($(this).val() === "teacher") {
            $groupSelect.hide();
            $schoolNumberInput.hide();
            $schoolNumberLabel.hide();
            $coursesDiv.show();
        } else {
            $groupSelect.hide();
            $schoolNumberInput.hide();
            $schoolNumberLabel.hide();
            $coursesDiv.hide();
        }
    });

});
