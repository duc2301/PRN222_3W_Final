using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResultDTO> UploadImageAsync(Stream fileStream, string fileName, string folder);
        Task<bool> DeleteImageAsync(string publicId);
    }
}
