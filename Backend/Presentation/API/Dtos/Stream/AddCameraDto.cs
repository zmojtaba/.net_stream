using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Backend.Application.Validation;
using Backend.Domain.Entities.Stream;

namespace Backend.Presentation.API.Dtos.Stream
{
    // public class AddCameraDto
    // {
    //     public string? CameraUrl { get; set; }
    //     public IFormFile? VideoFile { get; set; }
    //     public string[]? CameraPoints { get; set; }
    // }

    public class AddCameraDto
    {
        public string Name { get; set; } = string.Empty;

        [Required]
        [UrlOrFilePath]
        public string Url { get; set; } = string.Empty;
        public Angel Angle { get; set; }
        public Position Position { get; set; }
        public List<double> Segments { get; set; }
    }
}