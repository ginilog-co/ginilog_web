namespace Genilog_WebApi.Model.AdminsModel
{
    public class CompanyApplyDataModel
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? SurName { get; set; }
        public string? FirstName { get; set; }
        public string? PhoneNo { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyUserName { get; set; }
        public string? CompanyAddress { get; set; }
        public List<string>? CompanyType { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class CompanyApplyDataModelDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? SurName { get; set; }
        public string? FirstName { get; set; }
        public string? PhoneNo { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyUserName { get; set; }
        public string? CompanyAddress { get; set; }
        public List<string>? CompanyType { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class AddCompanyApply
    {
        public string? Email { get; set; }
        public string? SurName { get; set; }
        public string? FirstName { get; set; }
        public string? PhoneNo { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyUserName { get; set; }
        public string? CompanyAddress { get; set; }
        public List<string>? CompanyType { get; set; }
    }
}
