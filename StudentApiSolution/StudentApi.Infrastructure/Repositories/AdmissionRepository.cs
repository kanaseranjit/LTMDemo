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
    public class AdmissionRepository : IAdmissionRepository
    {
        private readonly ApplicationDbContext _context;
        public AdmissionRepository(ApplicationDbContext context) => _context = context;
        public async Task ConfirmAdmissionAsync(Admission admission)
        {
            try
            {
                _context.Admissions.Add(admission);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {

            }
        }


        public async Task<Admission?> GetByStudentIdAsync(int studentId)
        {
            Admission admissions= _context.Admissions.FirstOrDefault(x=>x.StudentId==studentId);
            return admissions;
        }
    }
}
