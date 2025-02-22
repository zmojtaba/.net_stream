using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Domain.Entities.Stream
{
    public class ExtractionTask
    {
        public string CameraUrl { get; set; }
        public Task Task { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public string Status { get; set; }
    }
}