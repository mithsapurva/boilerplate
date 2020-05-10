using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Document.API.Models
{
    public class FileDto
    {
        public virtual IFormFile UploadedFile { get; set; }

        public string ImageStorageName { get; set; }

        public string ImageUrl { get; set; }

        public string Title { get; set; }
    }
}
