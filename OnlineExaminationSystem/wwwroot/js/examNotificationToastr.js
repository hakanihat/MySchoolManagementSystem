
// Function to retrieve query parameters from URL
function getQueryParam(name) {
    var urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(name);
}

// Check if there is a success message in the query parameter
var successMessage = getQueryParam('successMessage');
if (successMessage) {
    // Display the custom alert
    showCustomAlert('EXAM NOTIFICATION!', successMessage);
}

function showCustomAlert(title, message) {
    var alertContainer = document.createElement('div');
    alertContainer.classList.add('custom-alert');

    var titleElement = document.createElement('div');
    titleElement.classList.add('alert-title');
    titleElement.textContent = title;

    var messageElement = document.createElement('div');
    messageElement.classList.add('alert-message');
    messageElement.textContent = message;

    alertContainer.appendChild(titleElement);
    alertContainer.appendChild(messageElement);

    document.body.appendChild(alertContainer);

    // Automatically remove the alert after 3 seconds
    setTimeout(function () {
        alertContainer.remove();
    }, 3000);
}
