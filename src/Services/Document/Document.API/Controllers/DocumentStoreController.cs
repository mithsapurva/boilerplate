using Document.API.DocumentStore;
using Document.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Document.API.Controllers
{
    [Route("api/[controller]")]
    public class DocumentStoreController : Controller
    {
        private readonly ICloudStorage cloudStorage;

        public DocumentStoreController(ICloudStorage storage)
        {
            cloudStorage = storage;
        }
       
        private async Task UploadFile(FileDto file)
        {
            string fileNameForStorage = FormFileName(file.Title, file.UploadedFile.FileName);
            file.ImageUrl = await cloudStorage.UploadFileAsync(file.UploadedFile, fileNameForStorage);
            file.ImageStorageName = fileNameForStorage;
        }

        private static string FormFileName(string title, string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            var fileNameForStorage = $"{title}-{DateTime.Now.ToString("yyyyMMddHHmmss")}{fileExtension}";
            return fileNameForStorage;
        }

        [HttpGet("{id}")]
        public string GetDocument(int id)
        {
            return "value";
        }
        
        [HttpPost]
        public async void Post([FromBody]FileDto file)
        {
            if(file.UploadedFile != null)
            {
                await UploadFile(file);
            }
        }

        [HttpDelete("{id}")]
        public async void Delete(FileDto file)
        {
            if (file.ImageStorageName != null)
            {
                await cloudStorage.DeleteFileAsync(file.ImageStorageName); 
            }
        }
    }
}