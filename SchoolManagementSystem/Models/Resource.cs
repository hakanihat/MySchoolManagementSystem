using SchoolManagementSystem.Data;
using System.Security.Claims;

namespace SchoolManagementSystem.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; }

        public int ClassId { get; set; }
        public virtual SchoolClass SchoolClass { get; set; }

        public string UploaderId { get; set; }
        public virtual ApplicationUser Uploader { get; set; }
    }

}
