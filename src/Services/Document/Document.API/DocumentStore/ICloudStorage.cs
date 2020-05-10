using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Document.API.DocumentStore
{
    public interface ICloudStorage
    {
        Task<string> UploadFileAsync(IFormFile file, string fileNameForStorage);
        Task DeleteFileAsync(string fileNameForStorage);
    }
}
