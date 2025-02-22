using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Application.Mappers.Stream;
using Backend.Application.Services.Stream;
using Backend.Domain.Entities.Stream;
using Backend.Infrustructure.Data;
using Backend.Presentation.API.Dtos.Stream;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic;

namespace Backend.Infrustructure.Respositories.Stream
{
    public class CameraRepository : ICameraRepository
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _currentTransaction;
        public CameraRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Camera> AddCameraAsync(AddCameraDto addCameraDto)
        {
            if (await _context.Cameras.AnyAsync(c => c.Url.ToLower() == addCameraDto.Url.ToLower()))
            {
                return null;
            }

            Camera camera = new Camera
            {
                Url = addCameraDto.Url,
                Name = addCameraDto.Name,
                Status = "Running",
                Angle = addCameraDto.Angle,
                Position = addCameraDto.Position,
                Segments = addCameraDto.Segments,
            };
            await _context.Cameras.AddAsync(camera);
            await _context.SaveChangesAsync();
            return camera;


        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCameraAsync(int cameraId)
        {
            Camera camera = await _context.Cameras.FindAsync(cameraId);
            _context.Cameras.Remove(camera);
            await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _currentTransaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _currentTransaction.RollbackAsync();
        }
    }
}