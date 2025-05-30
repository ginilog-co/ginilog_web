namespace Ginilog_AdminWeb.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
    public class ResponseModel
    {
        public string? Message { get; set; }
        public bool? Status { get; set; }
    }
}
