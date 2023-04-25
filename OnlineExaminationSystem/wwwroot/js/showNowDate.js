// Get the current date and time
var now = new Date();

// Format the date and time as an ISO string
var isoDateTimeString = now.toISOString().slice(0, 16).replace('T', ' ');

console.log(isoDateTimeString);

// Set the default value of the start time input field
document.getElementById('start-time-input').value = isoDateTimeString;

// Set the default value of the end time input field
document.getElementById('end-time-input').value = isoDateTimeString;
