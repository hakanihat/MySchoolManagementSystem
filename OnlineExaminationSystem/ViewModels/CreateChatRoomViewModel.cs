
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace OnlineExaminationSystem.ViewModels
{
    public class CreateChatRoomViewModel
    {
        [Required]
        public string Name { get; set; }
        public List<SelectListItem> ParticipantOptions { get; set; } = new List<SelectListItem>();
    }

}
