using Microsoft.AspNetCore.Mvc;
using OnlineExaminationSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OnlineExaminationSystem.Models;
using System.Security.Claims;
using OnlineExaminationSystem.ViewModels;
using System.Data.Common;

namespace OnlineExaminationSystem.Controllers
{
    public class ChatPanelController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SendGridEmailSender> _logger;

        public ChatPanelController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, ILogger<SendGridEmailSender> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var chatPanel = _dbContext.ChatPanels.Include(cp => cp.User)
                                                     .Include(cp => cp.ChatRooms)
                                                     .FirstOrDefault(cp => cp.ApplicationUserId == userId);

                if (chatPanel == null)
                {
                    return NotFound();
                }

                var chatRoomIds = chatPanel.ChatRooms.Select(cr => cr.Id).ToList();
                var chatRoomNames = chatPanel.ChatRooms.Select(cr => cr.Name).ToList();

                var chatPanelViewModel = new ChatPanelViewModel
                {
                    ChatRoomIds = chatRoomIds,
                    ChatRoomNames = chatRoomNames
                };

                return View("ChatPanelView", chatPanelViewModel);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "An InvalidOperationException occurred while retrieving the chat panel.");
                return RedirectToAction("Index", "Error");
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "A DbException occurred while retrieving the chat panel.");
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the chat panel.");
                return RedirectToAction("Index", "Error");
            }
        }



    }


}
