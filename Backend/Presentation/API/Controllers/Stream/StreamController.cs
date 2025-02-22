using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend.Application.Dtos;
using Backend.Application.Interfaces.Stream;
using Backend.Domain.Entities.Stream;
using Backend.Presentation.API.Dtos.Stream;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Presentation.API.Controllers.Stream
{
    [ApiController]
    [Route("api/stream")]

    public class StreamController : ControllerBase
    {

        private readonly IStreamService _streamService;
        private readonly ICameraRabbitService _iCRQService;

        public StreamController(IStreamService streamService, ICameraRabbitService cameraRabbitService)
        {
            _streamService = streamService;
            _iCRQService = cameraRabbitService;
        }

        [HttpPost]
        [Route("add-camera")]

        public async Task<IActionResult> AddCamera([FromBody] AddCameraDto addCameraDto)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            Response response = await _streamService.AddCamera(addCameraDto);

            // if (!ModelState.IsValid) return BadRequest(ModelState);

            // if (addCameraDto.VideoFile != null)
            // {

            //     string fileName = addCameraDto.VideoFile.FileName;

            //     // check that this file is video
            //     if (VideoTypes.Contains(Path.GetExtension(fileName), StringComparer.OrdinalIgnoreCase))
            //     {

            //         string videoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);
            //         //saving video in local
            //         using (FileStream videoStream = new FileStream(videoPath, FileMode.Create))
            //         {
            //             await addCameraDto.VideoFile.CopyToAsync(videoStream);
            //         }

            //         _streamService.ExtractFrames(videoPath);

            //     }


            // }

            return StatusCode(response.StatusCode, response.Message);
        }

        [HttpPost]
        [Route("delete-camera")]
        public async Task<IActionResult> DeleteCamera([FromBody] string url)
        {
            Console.WriteLine($"888-----{url}");
            Response response = await _streamService.StopCamera(url);
            return StatusCode(response.StatusCode, response.Message);


        }

        [HttpGet]
        [Route("send-message")]
        public async Task<IActionResult> SendMessage()
        {
            try
            {
                await _iCRQService.SendImagesAsBase64Async("camera_2", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                return Ok("ok");
            }
            catch (Exception ex)
            {
                return Ok(ex.ToString());
            }
        }
    }
}