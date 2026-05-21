using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using StudentApi.Application.Interfaces;
using StudentApi.Domain.Entities;
using ILogger = Serilog.ILogger;

namespace StudentApi.WebApi.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _repository;
        private readonly ILogger _logger;  // Serilog logger
        public StudentsController(IStudentRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var student = await _repository.GetAllAsync();
                _logger.Information("Students: " + student.Count());
                return (student == null) ? NotFound() : Ok(student);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching student List");
                return StatusCode(500, "Internal server error" + ex.Message + ex.InnerException);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            return (student == null) ? NotFound() : Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Student student)
        {
            await _repository.AddAsync(student);
            return Ok(student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Student student)
        {
            if (id != student.Id) return BadRequest();
            await _repository.UpdateAsync(student);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.SoftDeleteAsync(id);
            return NoContent();
        }
    }
}
