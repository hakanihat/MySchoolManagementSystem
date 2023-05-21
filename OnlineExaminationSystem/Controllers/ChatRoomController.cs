using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.ViewModels;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace OnlineExaminationSystem.Controllers
{
    public class ChatRoomController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatRoomController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int id)
        {
            var chatRoom = _dbContext.ChatRooms.Include(a => a.ApplicationUsers).FirstOrDefault(cr => cr.Id == id);

            if (chatRoom == null)
            {
                return NotFound(); // Return a 404 Not Found response if the chat room is not found
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var userWithProfile = _dbContext.Users.Include(u => u.UserProfile).FirstOrDefault(u => u.Id == currentUser.Id);
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string> { { GetRecipientId(chatRoom.ApplicationUsers.ToList(), currentUser).Keys.FirstOrDefault(), GetRecipientId(chatRoom.ApplicationUsers.ToList(), currentUser).Values.First() } };
            var viewModel = new ChatRoomViewModel
            {
                ChatRoomId = chatRoom.Id,
                SenderId = userWithProfile.Id,
                SenderFullName = userWithProfile.UserProfile.FullName,
                RecipientId = keyValuePairs.Keys.First(),
                RecipientFullName =keyValuePairs.Values.First()// Get the recipient ID based on the chat room's users

            };

            return View(viewModel);
        }

        private Dictionary<string, string> GetRecipientId(List<ApplicationUser> users, ApplicationUser cu)
        {
            // Replace this logic with your own to find the recipient ID

            foreach (var user in users)
            {
                if (user.Id != cu.Id)
                {
                    var userWithProfile = _dbContext.Users.Include(u => u.UserProfile).FirstOrDefault(u => u.Id == user.Id);
                    var dictionary = new Dictionary<string, string>();
                    return new Dictionary<string, string>  { { userWithProfile.Id, userWithProfile.UserProfile.FullName } };
                }
            }

            return null; // Return null if recipient ID is not found
        }


        public IActionResult Create()
        {
            var viewModel = new CreateChatRoomViewModel
            {
                ParticipantOptions = GetParticipantOptions()
            };

            return View("CreateChatRoomView", viewModel);
        }

        private List<SelectListItem> GetParticipantOptions()
        {
            // Retrieve the list of users/students from the database
            var users = _dbContext.Users.Include(u => u.UserProfile).ToList();

            // Create a list of SelectListItem objects for the dropdown
            var options = users.Select(user => new SelectListItem
            {
                Value = user.Id,
                Text = user.UserProfile.FullName
            }).ToList();

            return options;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateChatRoomViewModel model, string selectedParticipantIds)
        {
            if (ModelState.IsValid)
            {
                var chatRoom = new ChatRoom
                {
                    Name = model.Name,
                    ApplicationUsers = new List<ApplicationUser>(),
                    ChatPanels = new List<ChatPanel>()
                };

                // Add the current user's ID to the selected participant IDs if not already included
                var currentUser = await _userManager.GetUserAsync(User);
                var currentUserId = currentUser.Id;

                var selectedParticipantIdsJson = JsonConvert.DeserializeObject<HashSet<string>>(selectedParticipantIds);
                var participantIdSet = new HashSet<string>(selectedParticipantIdsJson)
        {
            currentUserId
        };

                // Process the selected participant IDs and save them in the database
                foreach (var participantId in participantIdSet)
                {
                    var chatRoomUser = new ChatRoomUser
                    {
                        ChatRoom = chatRoom,
                        UserId = participantId
                    };

                    _dbContext.ChatRoomUsers.Add(chatRoomUser);

                    // Retrieve the participant's chat panel
                    var participantChatPanel = _dbContext.ChatPanels
                        .Include(cp => cp.ChatRooms)
                        .FirstOrDefault(cp => cp.ApplicationUserId == participantId);

                    if (participantChatPanel == null)
                    {
                        // Create a new chat panel for the participant if it doesn't exist
                        participantChatPanel = new ChatPanel
                        {
                            ApplicationUserId = participantId,
                            User = await _userManager.FindByIdAsync(participantId),
                            ChatRooms = new List<ChatRoom>()
                        };

                        _dbContext.ChatPanels.Add(participantChatPanel);
                    }

                    // Add the chat room to the participant's chat panel
                    participantChatPanel.ChatRooms.Add(chatRoom);
                }

                _dbContext.ChatRooms.Add(chatRoom);
                _dbContext.SaveChanges();

                return RedirectToAction("Index", "ChatPanel");
            }

            return View("CreateChatRoomView", model);
        }





    }

}
