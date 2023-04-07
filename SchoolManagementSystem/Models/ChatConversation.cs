using SchoolManagementSystem.Data;

namespace SchoolManagementSystem.Models
{
    public class ChatConversation
    {
        public int Id { get; set; }
        public string User1Id { get; set; }
        public string User2Id { get; set; }

        public virtual ApplicationUser User1 { get; set; }
        public virtual ApplicationUser User2 { get; set; }
        public virtual ICollection<ChatMessage> Messages { get; set; }
    }
}
