using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain.Entities.Stream;
using Backend.Presentation.API.Dtos.Stream;

namespace Backend.Application.Mappers.Stream
{
    public static class CameraMappers
    {
        public static Camera ToCameraFromAdd(this AddCameraDto addCameraDto)
        {
            return new Camera
            {
                Url = addCameraDto.Url,
                Name = addCameraDto.Name,
                Status = "running",
                Angle = addCameraDto.Angle,
                Position = addCameraDto.Position,
                Segments = addCameraDto.Segments,
            };
        }

    }
}