namespace OnlineExaminationSystem.Models
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<ChatRoomUser> ChatRoomUsers { get; set; }
    }
}
