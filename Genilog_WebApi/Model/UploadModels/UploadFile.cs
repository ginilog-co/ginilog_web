using System.Text.Json.Serialization;

namespace Genilog_WebApi.Model.UploadModels
{
    public class UploadFile
    {
        public Guid Id { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? ImagePath { get; set; }
        public IFormFile? Image { get; set; }
    }
    public class ImageUploadResponse
    {
        public string? ImageUrl { get; set; }
    }
}
