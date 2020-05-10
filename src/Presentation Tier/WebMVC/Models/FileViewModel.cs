using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class FileViewModel
    {
        public virtual IFormFile UploadedFile { get; set; }

        public string ImageStorageName { get; set; }
    }
}
