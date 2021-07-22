namespace BlogProject.ViewModels
{
    public class MailSettings
    {
        // Configure and use smtp server
        // from google for example
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}