using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.ViewModels;
using Newtonsoft.Json;

namespace OnlineExaminationSystem.Controllers
{
    public class ChatRoomController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SendGridEmailSender> _logger;

        public ChatRoomController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, ILogger<SendGridEmailSender> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int id)
        {
            var chatRoom = _dbContext.ChatRooms.Include(a => a.ApplicationUsers).FirstOrDefault(cr => cr.Id == id);

            if (chatRoom == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var userWithProfile = _dbContext.Users.Include(u => u.UserProfile).FirstOrDefault(u => u.Id == currentUser.Id);

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
            {
                { GetRecipientId(chatRoom.ApplicationUsers.ToList(), currentUser).Keys.FirstOrDefault(),
                    GetRecipientId(chatRoom.ApplicationUsers.ToList(), currentUser).Values.First()
                }
            };

            var viewModel = new ChatRoomViewModel
            {
                ChatRoomId = chatRoom.Id,
                SenderId = userWithProfile.Id,
                SenderFullName = userWithProfile.UserProfile.FullName,
                RecipientId = keyValuePairs.Keys.First(),
                RecipientFullName = keyValuePairs.Values.First()
            };

            return View(viewModel);
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
            try
            {
                var users = _dbContext.Users.Include(u => u.UserProfile).ToList();

                var options = users.Select(user => new SelectListItem
                {
                    Value = user.Id,
                    Text = user.UserProfile.FullName
                }).ToList();

                return options;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the participant options.");
                return new List<SelectListItem>();
            }
        }



        [HttpPost]
        public async Task<IActionResult> Create(CreateChatRoomViewModel model, string selectedParticipantIds)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var chatRoom = new ChatRoom
                    {
                        Name = model.Name,
                        ApplicationUsers = new List<ApplicationUser>(),
                        ChatPanels = new List<ChatPanel>()
                    };

                    var currentUser = await _userManager.GetUserAsync(User);
                    var currentUserId = currentUser.Id;
                    var selectedParticipantIdsJson = JsonConvert.DeserializeObject<HashSet<string>>(selectedParticipantIds);
                    var participantIdSet = new HashSet<string>(selectedParticipantIdsJson) { currentUserId };

                    foreach (var participantId in participantIdSet)
                    {
                        var chatRoomUser = new ChatRoomUser
                        {
                            ChatRoom = chatRoom,
                            UserId = participantId
                        };

                        _dbContext.ChatRoomUsers.Add(chatRoomUser);

                        var participantChatPanel = _dbContext.ChatPanels
                            .Include(cp => cp.ChatRooms)
                            .FirstOrDefault(cp => cp.ApplicationUserId == participantId);

                        if (participantChatPanel == null)
                        {
                            participantChatPanel = new ChatPanel
                            {
                                ApplicationUserId = participantId,
                                User = await _userManager.FindByIdAsync(participantId),
                                ChatRooms = new List<ChatRoom>()
                            };

                            _dbContext.ChatPanels.Add(participantChatPanel);
                        }

                        participantChatPanel.ChatRooms.Add(chatRoom);
                    }

                    _dbContext.ChatRooms.Add(chatRoom);
                    _dbContext.SaveChanges();

                    return RedirectToAction("Index", "ChatPanel");
                }

                return View("CreateChatRoomView", model);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while saving data to the database.");
                return RedirectToAction("Index", "Error");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "An error occurred while processing JSON data.");
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return RedirectToAction("Index", "Error");
            }
        }

        private Dictionary<string, string> GetRecipientId(List<ApplicationUser> users, ApplicationUser cu)
        {
            foreach (var user in users)
            {
                if (user.Id != cu.Id)
                {
                    var userWithProfile = _dbContext.Users.Include(u => u.UserProfile).FirstOrDefault(u => u.Id == user.Id);
                    var dictionary = new Dictionary<string, string>();
                    return new Dictionary<string, string> { { userWithProfile.Id, userWithProfile.UserProfile.FullName } };
                }
            }
            return null;
        }
    }
}
