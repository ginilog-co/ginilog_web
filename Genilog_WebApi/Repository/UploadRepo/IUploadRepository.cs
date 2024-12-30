
using Genilog_WebApi.Model.UploadModels;

namespace Genilog_WebApi.Repository.UploadRepo
{
    public interface IUploadRepository
    {
        Task<UploadFile> SavePostImageAsync(UploadFile postRequest);

        Task<ImageListUpload> SaveListImageAsync(ImageListUpload files);
    }
}
