using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem.Services
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;


        public ChatHub(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public const string HubUrl = "/chathub"; // Define the URL for the hub

        public async Task SendMessage( string message, int crId, string sId,string sName)
        {
            var chatRoomId = crId;
            var senderId = sId;

            // Save the message to the database
            var newMessage = new Message
            {
                Text = message,
                Timestamp = DateTime.UtcNow,
                ChatRoomId = chatRoomId,
                SenderId = senderId
            };

            _dbContext.Messages.Add(newMessage);
            await _dbContext.SaveChangesAsync();

            // Broadcast the message to all clients in the chat room
            await Clients.All.SendAsync("ReceiveMessage", sName, message, chatRoomId,DateTime.Now);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            // Retrieve all messages for the chat room
            var chatRoomId = int.Parse(Context.GetHttpContext().Request.Query["chatRoomId"]);
            var messages = _dbContext.Messages.Include(m => m.Sender).ThenInclude(s => s.UserProfile)
                .Where(m => m.ChatRoomId == chatRoomId)
                .OrderBy(m => m.Timestamp)
                .ToList();

            // Send the messages to the connected client
            foreach (var message in messages)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage",
                    message.Sender.UserProfile.FullName,
                    message.Text,
                    message.ChatRoomId,
                    message.Timestamp);
            }
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Perform any necessary logic when a client is disconnected
            // For example, you can remove the user from connected users or groups

            await base.OnDisconnectedAsync(exception);
        }
    }
}
