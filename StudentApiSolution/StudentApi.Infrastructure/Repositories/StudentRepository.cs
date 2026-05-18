using Microsoft.EntityFrameworkCore;
using StudentApi.Application.Interfaces;
using StudentApi.Domain.Entities;
using StudentApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApi.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;
        public StudentRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Student>> GetAllAsync() => await _context.Students.Where(x=>!x.IsDeleted).ToListAsync();

        public async Task<Student?> GetByIdAsync(int id) => await _context.Students.FindAsync(id);

        public async Task AddAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            Student student = _context.Students.FirstOrDefault(x => x.Id == id);
            if (student != null)
            {
                student.IsDeleted = true;
            }
            _context.Students.Update(student);
            await _context.SaveChangesAsync();

        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }
    }
}
