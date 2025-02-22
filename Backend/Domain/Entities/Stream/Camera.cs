using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Domain.Entities.Stream
{
    public class Camera
    {
        public int CameraId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Angel Angle { get; set; } = new Angel();
        public Position Position { get; set; } = new Position();
        public List<double> Segments { get; set; } = new List<double>();
    }
}