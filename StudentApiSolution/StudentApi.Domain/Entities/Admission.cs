using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApi.Domain.Entities
{
    public class Admission
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public decimal FeesPaid { get; set; }
        public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;
        public bool IsConfirmed { get; set; } = false;
    }
}
