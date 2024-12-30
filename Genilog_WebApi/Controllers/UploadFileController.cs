
using Genilog_WebApi.Model.UploadModels;
using Genilog_WebApi.Repository.UploadRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Genilog_WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UploadFileController(IUploadRepository uploadRepository, IWebHostEnvironment _environment) : ControllerBase
    {
        private readonly IUploadRepository uploadRepository = uploadRepository;

        private readonly IWebHostEnvironment _environment = _environment;

        [Route("upload-image")]
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] AddUploadFile file)
        {

            var upload = new UploadFile()
            {
                Image = file.Image,
            };
            var img = await uploadRepository.SavePostImageAsync(upload);

            var userDto = new UploadeFileDto()
            {

                ImagePath = img.ImagePath,

            };

            return Ok(userDto);
        }

        [HttpPost]
        [Route("upload-image-list")]
        public async Task<IActionResult> AddImageListAsync([FromForm] AddImageList request)
        {
            var upload = new ImageListUpload()
            {
                Image = request.Image,
            };
            var img = await uploadRepository.SaveListImageAsync(upload);

            var userDto = new ImageListUploadDto()
            {
                ImagePath = img.ImagePath,

            };

            return Ok(userDto);

        }

        [HttpPost("upload-web-server")]
        public async Task<IActionResult> UploadImages(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // Generate a unique filename
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // Set the directory to save images
            var uploadPath = _environment.WebRootPath != null
         ? Path.Combine(_environment.WebRootPath, "uploads")
         : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadPath); // Ensure the directory exists

            var filePath = Path.Combine(uploadPath, fileName);

            // Save the file to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Create the URL for accessing the uploaded image
            var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

            return Ok(new ImageUploadResponse { ImageUrl = imageUrl });
        }
    
}
}
