using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Models
{
    public class ChatRoomUser
    {
        public int Id { get; set; }
        public virtual ChatRoom ChatRoom { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
