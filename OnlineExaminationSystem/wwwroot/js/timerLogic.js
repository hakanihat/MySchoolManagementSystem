var examDuration = @Model.ExamDuration * 60; // Convert to seconds

var x = setInterval(function () {
    var hours = Math.floor(examDuration / 3600);
    var minutes = Math.floor((examDuration % 3600) / 60);
    var seconds = Math.floor(examDuration % 60);

    // Update the timer element with the remaining time
    document.getElementById("hours").innerHTML = hours < 10 ? "0" + hours : hours;
    document.getElementById("minutes").innerHTML = minutes < 10 ? "0" + minutes : minutes;
    document.getElementById("seconds").innerHTML = seconds < 10 ? "0" + seconds : seconds;

    examDuration--;

    // If the countdown is finished, display "Time's Up" and submit the exam
    if (examDuration < 0) {
        clearInterval(x);
        document.getElementById("hours").innerHTML = "00";
        document.getElementById("minutes").innerHTML = "00";
        document.getElementById("seconds").innerHTML = "00";
        submitExam();
    }
}, 1000);

function submitExam() {
    var formData = $("#examForm").serializeArray();
    var selectedAnswers = {};
    var textAnswers = {};

    for (var i = 0; i < formData.length; i++) {
        var key = formData[i].name;
        var value = formData[i].value;

        if (key.endsWith("selectedAnswerId")) {
            var questionId = key.replace("questions[", "").replace("].selectedAnswerId", "");
            if (!selectedAnswers[questionId]) {
                selectedAnswers[questionId] = [value];
            } else {
                selectedAnswers[questionId].push(value);
            }
        } else if (key.endsWith("selectedAnswerIds")) {
            var questionId = key.replace("questions[", "").replace("].selectedAnswerIds", "");
            var answerIds = value.split(",");
            if (!selectedAnswers[questionId]) {
                selectedAnswers[questionId] = answerIds;
            } else {
                selectedAnswers[questionId] = selectedAnswers[questionId].concat(answerIds);
            }
        } else if (key.match(/^\d+$/)) {
            textAnswers[key] = value;
        }
    }

    var jsonData = JSON.stringify(selectedAnswers);
    console.log(jsonData);

    // Set the value of the hidden input fields to the JSON data
    $("#answersJson").val(jsonData);
    $("#textAnswersJson").val(JSON.stringify(textAnswers));

    // Submit the form
    document.getElementById("examForm").submit();
}

$(document).ready(function () {
    $("#submitExam").click(function () {
        submitExam();
    });
});