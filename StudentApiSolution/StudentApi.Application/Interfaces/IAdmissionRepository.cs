using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentApi.Domain.Entities;


namespace StudentApi.Application.Interfaces
{
    public interface IAdmissionRepository
    {
        Task<Admission?> GetByStudentIdAsync(int studentId);
        Task ConfirmAdmissionAsync(Admission admission);
    }
}
