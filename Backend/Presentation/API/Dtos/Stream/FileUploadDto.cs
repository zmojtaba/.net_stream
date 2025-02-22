using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Presentation.API.Dtos.Stream
{
    public class FileUploadDto
    {
        public List<IFormFile>? ImageFiles { get; set; }
        public string FileName { get; set; }
    }
}