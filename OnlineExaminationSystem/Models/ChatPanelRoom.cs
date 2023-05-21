namespace OnlineExaminationSystem.Models
{
    public class ChatPanelRoom
    {
        public int ChatPanelId { get; set; }
        public ChatPanel ChatPanel { get; set; }
        public int ChatRoomId { get; set; }
        public ChatRoom ChatRoom { get; set; }
    }
}
