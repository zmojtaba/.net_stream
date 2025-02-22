using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Application.Services.Stream
{
    public class CameraTaskManager
    {
        public ConcurrentDictionary<string, CancellationTokenSource> Tasks { get; } = new();
    }
}