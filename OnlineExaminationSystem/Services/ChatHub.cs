using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem.Services
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<SendGridEmailSender> _logger;


        public ChatHub(ApplicationDbContext dbContext, ILogger<SendGridEmailSender> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public const string HubUrl = "/chathub"; 

        public async Task SendMessage(string message, int crId, string sId, string sName)
        {
            var chatRoomId = crId;
            var senderId = sId;
            var newMessage = new Message
            {
                Text = message,
                Timestamp = DateTime.UtcNow,
                ChatRoomId = chatRoomId,
                SenderId = senderId
            };
            _dbContext.Messages.Add(newMessage);
            await _dbContext.SaveChangesAsync();
            await Clients.All.SendAsync("ReceiveMessage", sName, message, chatRoomId, DateTime.Now);
        }


        public override async Task OnConnectedAsync()
        {
            try
            {
                await base.OnConnectedAsync();

                // Retrieve all messages for the chat room
                var chatRoomIdStr = Context.GetHttpContext().Request.Query["chatRoomId"];

                if (int.TryParse(chatRoomIdStr, out var chatRoomId))
                {
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
                else
                {
                    _logger.LogError("Invalid chatRoomId: {chatRoomIdStr}", chatRoomIdStr);
                }
            }
            catch (Exception ex)
            {
              
                _logger.LogError(ex, "An error occurred while retrieving and sending messages.");
                throw;
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
