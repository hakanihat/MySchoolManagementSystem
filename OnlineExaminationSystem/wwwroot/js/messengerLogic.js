document.addEventListener("DOMContentLoaded", function () {
    // Create a connection to the SignalR hub
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub?chatRoomId=" + "@Model.ChatRoomId") // Replace with the actual URL of the ChatHub
        .build();

    function sendMessageToServer(message, chatRoomId, senderId, senderFullName) {
        connection.invoke("SendMessage", message, parseInt(chatRoomId), senderId, senderFullName)
            .catch(function (error) {
                console.error("Error sending message: " + error);
            });
    }

    // Start the connection
    connection.start()
        .then(function () {
            console.log("Connected to the chat hub.");
        })
        .catch(function (error) {
            console.error("Error connecting to the chat hub: " + error);
        });

    // Event listener for sending a message
    document.getElementById("send-button").addEventListener("click", function (event) {
        event.preventDefault(); // Prevent form submission
        var message = document.getElementById("message-input").value;
        if (message.trim() !== "") {
            // Send the message to the server through the hub
            sendMessageToServer(message, "@Model.ChatRoomId", "@Model.SenderId", "@Model.SenderFullName");
            document.getElementById("message-input").value = ""; // Clear the input field
        }
    });

    // Event handler for receiving a message from the server
    connection.on("ReceiveMessage", function (user, message, chatRoom, date) {
        // Create a message container and append it to the chat box
        var messageContainer = document.createElement("div");
        messageContainer.className = "message-container";
        var messageSender = document.createElement("div");
        messageSender.className = "message-sender";

        // Apply different class based on the sender
        if (user === "@Model.SenderFullName") {
            messageSender.classList.add("me");
        } else {
            messageSender.classList.add("other");
        }

        var formattedDate = new Date(date).toLocaleString('en-US', {
            day: 'numeric',
            month: 'short',
            year: 'numeric',
            hour: 'numeric',
            minute: 'numeric',
            hour12: true
        });

        messageSender.innerText = user + " - " + formattedDate;
        var messageBubble = document.createElement("div");
        messageBubble.className = "message-bubble";

        // Apply different class based on the sender
        if (user === "@Model.SenderFullName") {
            messageBubble.classList.add("me");
        } else {
            messageBubble.classList.add("other");
        }

        messageBubble.innerText = message;

        messageContainer.appendChild(messageSender);
        messageContainer.appendChild(messageBubble);
        document.getElementById("chat-box-@Model.ChatRoomId").appendChild(messageContainer);
        var chatBox = document.getElementById("chat-box-@Model.ChatRoomId");
        chatBox.scrollTop = chatBox.scrollHeight;
    });

    document.getElementById("message-input").addEventListener("keydown", function (event) {
        if (event.key === "Enter" && !event.shiftKey) {
            event.preventDefault(); // Prevent the default Enter key behavior (e.g., new line)
            var message = document.getElementById("message-input").value;
            if (message.trim() !== "") {
                sendMessageToServer(message, "@Model.ChatRoomId", "@Model.SenderId", "@Model.SenderFullName");
                document.getElementById("message-input").value = ""; // Clear the input field
            }
        }
    });
});