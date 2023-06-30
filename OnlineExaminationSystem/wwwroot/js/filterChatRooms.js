// Function to filter and sort the chat rooms based on the search input
function filterChatRooms() {
    var searchInput = document.getElementById('search-input').value.toLowerCase();
    var chatRooms = document.querySelectorAll('.list-group-item');

    chatRooms.forEach(function (chatRoom) {
        var chatRoomName = chatRoom.querySelector('.participant-name').textContent.toLowerCase();

        if (chatRoomName.indexOf(searchInput) > -1) {
            chatRoom.style.display = '';
        } else {
            chatRoom.style.display = 'none';
        }
    });
}

// Event listener for the search input
document.getElementById('search-input').addEventListener('input', function () {
    filterChatRooms();
});