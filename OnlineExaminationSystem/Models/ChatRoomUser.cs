namespace OnlineExaminationSystem.Models
{
    public class ChatRoomUser
    {
        public int RoomId { get; set; }
        public virtual ChatRoom ChatRoom { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
