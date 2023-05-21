namespace OnlineExaminationSystem.ViewModels
{
    public class ChatRoomViewModel
    {
        public int ChatRoomId { get; set; }
        public string SenderId { get; set; }
        public string SenderFullName { get; set; }

        public string RecipientId { get; set; }
        public string RecipientFullName { get; set; }
    }

}
