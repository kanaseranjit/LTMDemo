using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentApi.Domain.Entities;

namespace StudentApi.Infrastructure.Data
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<Admission> Admissions { get; set; }
        public DbSet<StudentApi.Domain.Entities.Stream> Streams { get; set; }
        public DbSet<Student> Students { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // Seed Streams
        //    modelBuilder.Entity<StudentApi.Domain.Entities.Stream>().HasData(
        //        new StudentApi.Domain.Entities.Stream { Id = 1, Name = "Arts" },
        //        new StudentApi.Domain.Entities.Stream { Id = 2, Name = "Commerce" },
        //        new StudentApi.Domain.Entities.Stream { Id = 3, Name = "Science" },
        //        new StudentApi.Domain.Entities.Stream { Id = 4, Name = "Engineering" }
        //    );
        //}
    }
}
