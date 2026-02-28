using Firebase.Storage;
using Genilog_WebApi.Key;
using Genilog_WebApi.Model.UploadModels;
using static Google.Rpc.Help.Types;

namespace Genilog_WebApi.Repository.UploadRepo
{
    public class UploadRepository(IWebHostEnvironment _environment) : IUploadRepository
    {
        private readonly IWebHostEnvironment _environment=_environment;

        public async Task<ImageListUpload> SaveListImageAsync(ImageListUpload files)
        {
            List<string> path = new([]);

            string link = "";
            FileStream? fs = null;
            if (files.Image!.Count == 0)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            var token = Guid.NewGuid();
            foreach (var file in files.Image!)
            {
                var uniqueFileName = FileHelper.GetUniqueFileName(file!.FileName);
                var extension = Path.GetExtension(file.FileName);
                string fileName = DateTime.UtcNow.Ticks + uniqueFileName;
                using (var memoyrStream = new FileStream(Path.Combine(fileName), FileMode.Create))
                {
                    await file.CopyToAsync(memoyrStream);
                }
                using (fs = new FileStream(Path.Combine(fileName), FileMode.Open))
                {
                    var cancellation = new CancellationTokenSource();
                    var upload = new FirebaseStorage(
                          Cls_Keys.BucketFile,
                           new FirebaseStorageOptions
                           {
                               AuthTokenAsyncFactory = () => Task.FromResult(token.ToString()),
                               ThrowOnCancel = true
                           })
                           .Child("ListUploadFolder")
                          .Child(token.ToString())
                           .Child(fileName)
                           .PutAsync(fs, cancellation.Token);

                    // error during upload will be thrown when await the task
                    link = await upload;
                }

                path.Add(link);


            }

            files.ImagePath = path;

            return files;
        }

        public async Task<UploadFile> SavePostImageAsync(UploadFile postRequest)
        {
            string link = "";
            FileStream? fs = null;
            var token = Guid.NewGuid();
            var uniqueFileName = FileHelper.GetUniqueFileName(postRequest.Image!.FileName);

            string fileName = DateTime.UtcNow.Ticks + uniqueFileName;

            using (var memoyrStream = new FileStream(Path.Combine(fileName), FileMode.Create))
            {
                await postRequest.Image.CopyToAsync(memoyrStream);
            }
            using (fs = new FileStream(Path.Combine(fileName), FileMode.Open))
            {
                var cancellation = new CancellationTokenSource();
                var upload = new FirebaseStorage(
                      Cls_Keys.BucketFile,
                       new FirebaseStorageOptions
                       {
                           AuthTokenAsyncFactory = () => Task.FromResult(token.ToString()),
                           ThrowOnCancel = true
                       })
                .Child("FileReposiotory")
                      .Child(token.ToString())
                       .Child(fileName)
                       .PutAsync(fs, cancellation.Token);

                // error during upload will be thrown when await the task
                link = await upload;
            }
            postRequest.ImagePath = link;

            return postRequest;
        }
        public async Task<UploadFile> SaveServerImageAsync(UploadFile postRequest)
        {

            if (postRequest.Image == null || postRequest.Image!.Length == 0)
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.

            // Generate a unique filename
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(postRequest.Image!.FileName);

            // Set the directory to save images
            var uploadPath = _environment.WebRootPath != null
         ? Path.Combine(_environment.WebRootPath, "uploads")
         : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadPath); // Ensure the directory exists

            var filePath = Path.Combine(uploadPath, fileName);

            // Save the file to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await postRequest.Image!.CopyToAsync(stream);
            }

            // Create the URL for accessing the uploaded image
            var imageUrl = $"{fileName}";
            postRequest.ImagePath = imageUrl;
            return postRequest;
        }
    }
}
