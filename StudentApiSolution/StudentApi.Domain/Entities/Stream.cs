using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApi.Domain.Entities
{
    public class Stream
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Arts, Commerce, Science, Engineering
        public ICollection<Student> Students { get; set; } = new List<Student>();

    }
}
