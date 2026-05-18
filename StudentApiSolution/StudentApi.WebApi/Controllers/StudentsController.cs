using Microsoft.AspNetCore.Mvc;
using StudentApi.Application.Interfaces;
using StudentApi.Domain.Entities;

namespace StudentApi.WebApi.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _repository;
        public StudentsController(IStudentRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var student = await _repository.GetAllAsync();

            return (student == null) ? NotFound() : Ok(student);
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
