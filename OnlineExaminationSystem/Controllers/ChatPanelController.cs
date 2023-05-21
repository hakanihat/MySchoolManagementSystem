using Microsoft.AspNetCore.Mvc;
using OnlineExaminationSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OnlineExaminationSystem.Models;
using System.Security.Claims;
using OnlineExaminationSystem.ViewModels;

namespace OnlineExaminationSystem.Controllers
{
    public class ChatPanelController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatPanelController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
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

            return View("ChatPanelView",chatPanelViewModel);
        }


    }


}
