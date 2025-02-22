using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Application.Interfaces.Stream
{
    public interface ICameraRabbitService
    {
        public Task SendImagesAsBase64Async(string queueName, string base64Image);
    }
}