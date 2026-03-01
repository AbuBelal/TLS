using SharedLib.Entities;
using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Responses;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;

        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<ActionResult<List<Student>>> GetAll()
        {
            var result = await _studentRepository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetById(long id)
        {
            var result = await _studentRepository.GetById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Insert(Student student)
        {
            var response = await _studentRepository.Insert(student);
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<GeneralResponse>> Update(Student student)
        {
            var response = await _studentRepository.Update(student);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> Delete(long id)
        {
            var response = await _studentRepository.DeleteById(id);
            return Ok(response);
        }
    }
}