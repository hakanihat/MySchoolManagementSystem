// Function to filter and sort the questions based on the search input
function filterQuestions() {
    var searchInput = document.getElementById('search-input').value.toLowerCase();
    var questions = document.querySelectorAll('.table tbody tr');

    questions.forEach(function (question) {
        var questionText = question.querySelector('td:first-child').textContent.toLowerCase();

        if (questionText.indexOf(searchInput) > -1) {
            question.style.display = '';
        } else {
            question.style.display = 'none';
        }
    });
}

// Event listener for the search input
document.getElementById('search-input').addEventListener('input', function () {
    filterQuestions();
});
