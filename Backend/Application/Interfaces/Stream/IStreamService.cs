using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Backend.Application.Dtos;
using Backend.Presentation.API.Dtos.Stream;

namespace Backend.Application.Interfaces.Stream
{
    public interface IStreamService
    {

        public Task<Response> AddCamera(AddCameraDto addCameraDto);
        public Task ExtractFrames(string queueName, string url, CancellationToken cts);

        public Task<Response> StopCamera(string url);


    }
}