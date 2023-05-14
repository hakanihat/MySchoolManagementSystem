$(function () {
    var $container = $('#create-exam-form');
    var $selectedQuestions = $('select[name="SelectedQuestionIds"]');
    var questionInputs = [];

    function addQuestionInput(questionId, questionText) {
        // create the text input with name and value attributes
        var inputName = 'SelectedQuestionIds[' + questionInputs.length + ']';
        var $questionInput = $('<input>').attr('type', 'text').addClass('form-control').prop('readonly', true).attr('name', inputName).val(questionText);

        // create the delete button
        var $deleteButton = $('<button>').addClass('btn btn-danger').text('Delete');

        // create the input container
        var $inputContainer = $('<div>').addClass('form-group input-container').append($questionInput).append($deleteButton);
        $inputContainer.data('question-id', questionId);

        // add the delete button event handler
        $deleteButton.click(function () {
            $inputContainer.remove();
            questionInputs.splice(questionInputs.indexOf($inputContainer), 1);
            updateSelectedQuestionIds();
        });

        // insert the input container before the create button
        $container.find('#create-exam-inputs').before($inputContainer);

        // add the new input to the questionInputs array
        questionInputs.push($inputContainer);

        // update the selected question IDs list
        updateSelectedQuestionIds();
    }


    function updateSelectedQuestionIds() {
        var selectedQuestionIds = [];
        questionInputs.forEach(function ($inputContainer) {
            var questionId = $inputContainer.data('question-id');
            selectedQuestionIds.push(questionId);
        });
        $('#SelectedQuestionIdsJson').val(JSON.stringify(selectedQuestionIds));
    }


    $selectedQuestions.change(function () {
        var selectedQuestionId = $selectedQuestions.val();
        var selectedQuestionText = $selectedQuestions.find('option:selected').text();

        if (selectedQuestionId && selectedQuestionText) {
            var alreadySelected = false;
            for (var i = 0; i < questionInputs.length; i++) {
                if (questionInputs[i].data('question-id') === selectedQuestionId) {
                    alreadySelected = true;
                    break;
                }
            }

            if (alreadySelected) {
                var errorMessageContainer = $selectedQuestions.closest('.form-group');
                var errorMessage = errorMessageContainer.find('.error-message');

                if (errorMessage.length === 0) {
                    errorMessage = $('<span>').addClass('error-message').text('This question has already been selected.');
                    errorMessageContainer.append(errorMessage);
                }
            } else {
                addQuestionInput(selectedQuestionId, selectedQuestionText);
                $selectedQuestions.val('');
                $selectedQuestions.closest('.form-group').find('.error-message').remove();
            }
        }
    });


    $('#generate-questions-button').click(function (event) {
        event.preventDefault();
        console.log('Button clicked');
        var numQuestions = parseInt($('#num-questions-input').val());
        if (isNaN(numQuestions) || numQuestions <= 0) {
            showMessage('Please enter a valid number of questions.', 'error');
            return;
        }
        var courseId = $('#course-dropdown').val();
        console.log('Course ID:', courseId);
        if (!courseId) {
            showMessage('Please select a course.', 'error');
            return;
        }

        $('#message-container').empty();


        $.getJSON('/Exam/GetQuestions', { courseId: courseId }, function (data) {
            var questions = data;
            console.log('Questions:', questions);

            if (questions.length < numQuestions) {
                showMessage('Not enough questions available for this course.', 'warning');
                return;
            }
            // shuffle the questions array
            questions = shuffleArray(questions);
            // select the first numQuestions questions
            questions = questions.slice(0, numQuestions);
            console.log('Selected Questions:', questions);

            // remove previously generated input fields
            $('#questions-container').empty();

            // iterate over the selected questions and add a question input for each one
            questions.forEach(function (question) {
                var alreadySelected = false;
                for (var i = 0; i < questionInputs.length; i++) {
                    if (questionInputs[i].data('question-id') === question.value) {
                        alreadySelected = true;
                        break;
                    }
                }
                if (!alreadySelected) {
                    addQuestionInput(question.value, question.text);
                }
            });
        }).fail(function (error) {
            showMessage('An error occurred while getting the questions. Please try again later.', 'error');
            console.log('Error: ', error);
        });
    });

    function showMessage(message, type) {
        var messageContainer = $('#message-container');
        messageContainer.empty();
        var messageElement = $('<div></div>');
        messageElement.addClass('message');
        messageElement.addClass(type);
        messageElement.text(message);
        messageContainer.append(messageElement);
    }


    function shuffleArray(array) {
        var currentIndex = array.length, temporaryValue, randomIndex;

        // While there remain elements to shuffle...
        while (0 !== currentIndex) {

            // Pick a remaining element...
            randomIndex = Math.floor(Math.random() * currentIndex);
            currentIndex -= 1;

            // And swap it with the current element.
            temporaryValue = array[currentIndex];
            array[currentIndex] = array[randomIndex];
            array[randomIndex] = temporaryValue;
        }

        return array;
    }




});
