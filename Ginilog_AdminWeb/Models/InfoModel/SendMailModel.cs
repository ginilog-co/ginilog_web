namespace Ginilog_AdminWeb.Models.InfoModel
{
    public class SendMailModel
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? Link { get; set; }
        public Guid UserId { get; set; }
    }
    public class SendMailModel2
    {
        public string? Link { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
    }
}
