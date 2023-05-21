namespace OnlineExaminationSystem.Models
{
    public class ChatPanel
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<ChatRoom> ChatRooms { get; set; }
    }


}
