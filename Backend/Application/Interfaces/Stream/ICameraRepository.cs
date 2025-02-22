using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain.Entities.Stream;
using Backend.Presentation.API.Dtos.Stream;

namespace Backend.Application.Services.Stream
{
    public interface ICameraRepository
    {
        public Task<Camera> AddCameraAsync(AddCameraDto addCameraDto);
        public Task BeginTransactionAsync();
        public Task CommitTransactionAsync();

        public Task SaveChangesAsync();
        public Task RollbackTransactionAsync();

        public Task DeleteCameraAsync(int cameraId);

    }
}