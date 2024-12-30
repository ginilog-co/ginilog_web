using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Genilog_WebApi.Model.UploadModels
{
    public class ImageListUpload
    {
        public Guid Id { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public List<string>? ImagePath { get; set; }
        [BindProperty]
        public List<IFormFile>? Image { get; set; }

    }

    public class ImageListUploadDto
    {
        public List<string>? ImagePath { get; set; }
    }
}
