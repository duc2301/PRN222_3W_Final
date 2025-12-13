using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RecipeSharingPlatform.Service.Configuration;
using RecipeSharingPlatform.Service.DTOs.ResponseDTOs;
using RecipeSharingPlatform.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingPlatform.Service.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                throw new Exception("Cloudinary configuration is missing. Please check appsettings.json.");
            }

            // chỉ để bạn tự tin là cloudName đã đúng
            // Console.WriteLine($"[Cloudinary] CloudName = {cloudName}");

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<ImageUploadResultDTO> UploadImageAsync(Stream fileStream, string fileName, string folder)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                throw new Exception($"Cloudinary upload error: {result.Error.Message}");
            }

            return new ImageUploadResultDTO
            {
                Url = result.SecureUrl.ToString()
            };
        }
        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId)
            {
                Invalidate = true
            };

            var result = await _cloudinary.DestroyAsync(deletionParams);

            return result.StatusCode == HttpStatusCode.OK &&
                   (result.Result == "ok" || result.Result == "not found");
        }
    }
}
