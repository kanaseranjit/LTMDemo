using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stream = StudentApi.Domain.Entities.Stream;

namespace StudentApi.Application.Interfaces
{
    public interface IStreamRepository
    {
        Task<IEnumerable<Stream>> GetAllAsync();
        Task<Stream?> GetByIdAsync(int id);
    }
}
