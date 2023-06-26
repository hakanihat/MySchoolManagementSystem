namespace OnlineExaminationSystem.Services
{
    public class SendGridEmailSenderOptions
    {
        public string ApiKey { get;private set; }
        public string SenderEmail { get;private set; }
        public string SenderName { get;private set; }
    }

}
