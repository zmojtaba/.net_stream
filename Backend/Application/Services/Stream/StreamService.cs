using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend.Application.Dtos;
using Backend.Application.Interfaces.Stream;
using Backend.Domain.Entities.Stream;
using Backend.Infrustructure.Data;
using Backend.Presentation.API.Dtos.Stream;
using IOStream = System.IO.Stream;


namespace Backend.Application.Services.Stream
{
    public class StreamService : IStreamService
    {
        // private List<ExtractionTask> _tasks = new List<ExtractionTask>();

        private readonly ApplicationDbContext _context;
        private string _cameraStatus;
        private string _ErrorMessage;
        private readonly ICameraRepository _cameraRepo;
        private readonly CameraTaskManager _taskManager;
        private readonly ICameraRabbitService _cameraRQService;
        private string[] VideoTypes = { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm" };

        public StreamService(ApplicationDbContext context, ICameraRepository cameraRepository, CameraTaskManager taskManager, ICameraRabbitService cameraRabbitService)
        {
            _context = context;
            _cameraRepo = cameraRepository;
            _taskManager = taskManager;
            _cameraRQService = cameraRabbitService;
        }

        public bool IsRtsp(string url)
        {
            if (url.StartsWith("rtsp://", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }



        public async Task<Response> AddCamera(AddCameraDto addCameraDto)
        {

            if (!IsRtsp(addCameraDto.Url))
            {
                string urlExtension = Path.GetExtension(addCameraDto.Url);

                if (urlExtension == null)
                    return new Response
                    {
                        StatusCode = 400,
                        Message = "Url is not a valid file path!"
                    };

                if (!VideoTypes.Contains(urlExtension, StringComparer.OrdinalIgnoreCase))
                    return new Response
                    {
                        StatusCode = 400,
                        Message = "Only video files with type of .mp4, .avi, .mkv, .mov, .wmv, .flv, .webm are valid! "
                    };

                // if (!File.Exists(addCameraDto.Url))
                //     return new Response
                //     {
                //         StatusCode = 404,
                //         Message = "File does not exists!"
                //     };
            }
            try
            {
                Camera camera = await _cameraRepo.AddCameraAsync(addCameraDto);

                if (camera == null) return new Response
                {
                    StatusCode = 400,
                    Message = "Camera with this url aleady exists!"
                };

                var cts = new CancellationTokenSource();

                _taskManager.Tasks[addCameraDto.Url] = cts;


                var task = Task.Run(() => ExtractFrames("camera_" + Convert.ToString(camera.CameraId), addCameraDto.Url, cts.Token), cts.Token)
                    .ContinueWith(async t =>
                    {
                        if (t.IsCanceled)
                        {
                            _cameraStatus = "Cancelled!";
                        }
                        else if (t.IsFaulted)
                        {
                            _cameraStatus = "Failed";
                            _ErrorMessage = t.Exception?.GetBaseException().Message;

                        }
                        else if (t.IsCompletedSuccessfully)
                        {

                            _cameraStatus = "Completed!";

                        }
                        // await _cameraRepo.SaveChangesAsync();
                        _taskManager.Tasks.TryRemove(addCameraDto.Url, out var _);
                    });


                await Task.Delay(3000);
                if (_cameraStatus != null)
                {

                    if (_cameraStatus.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                    {
                        await _cameraRepo.DeleteCameraAsync(camera.CameraId);
                        return new Response
                        {
                            StatusCode = 500,
                            Message = _ErrorMessage
                        };
                    }
                }

                return new Response
                {
                    StatusCode = 200,
                    Message = camera
                };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    StatusCode = 500,
                    Message = ex.ToString()
                };


            }


        }

        public async Task ExtractFrames(string queueName, string url, CancellationToken token)
        {

            await Task.Run(async () =>
            {
                using (Process ffmpeg = new Process())
                {


                    ffmpeg.StartInfo.FileName = "ffmpeg";
                    ffmpeg.StartInfo.Arguments = $"-hwaccel cuda -re -i \"{url}\" -f image2pipe -vcodec mjpeg -r 15 -vf scale=1024:1024 -an -";
                    ffmpeg.StartInfo.RedirectStandardOutput = true;
                    ffmpeg.StartInfo.RedirectStandardError = true;
                    ffmpeg.StartInfo.UseShellExecute = false;
                    ffmpeg.StartInfo.CreateNoWindow = true;

                    DateTime start = DateTime.Now;
                    ffmpeg.Start();

                    List<byte> imageBuffer = new List<byte>();
                    byte[] buffer = new byte[4096];

                    using (BinaryReader reader = new BinaryReader(ffmpeg.StandardOutput.BaseStream))
                    {
                        while (!token.IsCancellationRequested)
                        {
                            int bytesRead = reader.Read(buffer, 0, buffer.Length);
                            if (bytesRead == 0) break; // No more data, exit loop

                            imageBuffer.AddRange(buffer.Take(bytesRead));


                            // Detect JPEG end marker (0xFFD9)
                            if (imageBuffer.Count > 2 && imageBuffer[^2] == 0xFF && imageBuffer[^1] == 0xD9)
                            {
                                string base64Image = Convert.ToBase64String(imageBuffer.ToArray());
                                await _cameraRQService.SendImagesAsBase64Async(queueName, base64Image);

                                imageBuffer.Clear(); // Reset for next image
                            }
                        }
                    }

                    DateTime finish = DateTime.Now;

                    Console.WriteLine($"***********************************{finish - start}");

                    await ffmpeg.WaitForExitAsync();

                    if (ffmpeg.ExitCode != 0)
                    {
                        throw new Exception($"FFmpeg failed with exit code {ffmpeg.ExitCode}. Error: {ffmpeg.StandardError}");
                    }



                }
            });



        }

        public async Task<Response> StopCamera(string url)
        {
            if (!_taskManager.Tasks.ContainsKey(url))
                return new Response
                {
                    StatusCode = 404,
                    Message = "This camera is not running!"
                };
            if (_taskManager.Tasks.TryRemove(url, out var cts))
            {
                cts.Cancel();
                return new Response
                {
                    StatusCode = 200,
                    Message = "Camera has been deleted successfully!"
                };
            }
            return new Response
            {
                StatusCode = 500,
                Message = "Some things heppend!"
            };
        }
    }
}