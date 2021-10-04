using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary cloud;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
             var acc=new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            cloud= new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var upload=new ImageUploadResult();
            if(file.Length>0)
            {
                using var stream=file.OpenReadStream();
                var uploadparams= new ImageUploadParams{
                    File= new FileDescription(file.FileName,stream),
                    Transformation= new Transformation().Height(500).Width(500)
                };
                upload= await cloud.UploadAsync(uploadparams);
                
            }
            return upload;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams= new DeletionParams(publicId);
            var result= await cloud.DestroyAsync(deleteParams);
            return result;
        }
    }
}