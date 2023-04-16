
namespace SchoolManagementSystem.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }

        public int ChatRoomId { get; set; }
        public virtual ChatRoom ChatRoom { get; set; }

        public string SenderId { get; set; }
        public virtual ApplicationUser Sender { get; set; }
    }

}
