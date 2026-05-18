using Microsoft.EntityFrameworkCore;
using StudentApi.Application.Interfaces;
using StudentApi.Domain.Entities;
using StudentApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stream = StudentApi.Domain.Entities.Stream;

namespace StudentApi.Infrastructure.Repositories
{
    internal class StreamRepository : IStreamRepository
    {
        private readonly ApplicationDbContext _context;
        public StreamRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<StudentApi.Domain.Entities.Stream>> GetAllAsync() => await _context.Streams.ToListAsync();
        
        public async Task<Stream?> GetByIdAsync(int id) => await _context.Streams.FindAsync(id);
    }
}
