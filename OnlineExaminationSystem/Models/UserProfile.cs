using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineExaminationSystem.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? ContactInfo { get; set; }
        public string? Bio { get; set; }
        public string? PictureUrl { get; set; }

        [NotMapped]
        public IFormFile? PictureFile { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
