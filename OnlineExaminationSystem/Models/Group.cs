using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OnlineExaminationSystem.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
      
        public virtual ICollection<ApplicationUser> Students { get; set; } = new List<ApplicationUser>();
    }
}
