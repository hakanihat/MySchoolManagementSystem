function viewChatRoom(chatRoomId) {
    var url = '@Url.Action("Index", "ChatRoom")' + '?id=' + chatRoomId;
    window.location.href = url;
}