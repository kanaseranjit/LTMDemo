using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApi.Domain.Entities
{
    
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }        
        public string Email { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false; // Soft delete flag

        //public int StreamId { get; set; }
        //public Stream Stream { get; set; } = null!;
        //public Admission Admission { get; set; } = null!;
    }
}
