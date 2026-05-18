using Microsoft.AspNetCore.Mvc;
using StudentApi.Application.Interfaces;
using StudentApi.Domain.Entities;
using StudentApi.Infrastructure.Repositories;

namespace StudentApi.WebApi.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AdmissionsController : ControllerBase
    {
        private readonly IAdmissionRepository _repository;
        public AdmissionsController(IAdmissionRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmAdmission(int studentId, decimal feesPaid)
        {
            var admission = new Admission
            {
                StudentId = studentId,
                FeesPaid = feesPaid,
                IsConfirmed = true,
                AdmissionDate= DateTime.UtcNow
            };

            await _repository.ConfirmAdmissionAsync(admission);
            return Ok();
        }

    }
}
