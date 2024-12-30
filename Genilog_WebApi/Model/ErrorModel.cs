namespace Genilog_WebApi.Model
{
    public class ErrorModel
    {
        public string? Message { get; set; }
        public bool? Status { get; set; }
    }
    public class LoginErrorModel
    {
        public string? Message { get; set; }
        public bool? Status { get; set; }
        public string? AdminType { get; set; }

    }

    public class ResponseModel
    {
        public string? Message { get; set; }
        public bool? Status { get; set; }
    }
}
