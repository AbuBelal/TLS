using APIServerLib.Repositories.Interfaces;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;
using System.Security.Claims;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public StudentController(IStudentRepository studentRepository, IUserRepository UserRepository , IEmployeeRepository EmployeeRepository)
        {
            _studentRepository = studentRepository;
            _userRepository = UserRepository;
            _employeeRepository = EmployeeRepository;
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var FullUser = await _userRepository.GetById(userId);
            var Employee = await _employeeRepository.GetById(FullUser.EmployeeId ?? 0);

            if (Employee is not null)
            { 
                var centerid= Employee.EmpCenters.OrderByDescending(ec => ec.FromDate).FirstOrDefault()?.CenterId;
                var response = await _studentRepository.AddStudentWithCenter(student , centerid??0);

              return Ok(response);
            }
            return BadRequest();
        }

        //[HttpPost("AddWithCenter")]
        //public async Task<ActionResult<GeneralResponse>> AddWithCenter(StdWithCenterId student)
        //{
        //    var response = await _studentRepository.AddStudentWithCenter(student);
        //    return Ok(response);
        //}

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

        [HttpGet("StudentCenterCount/{id}")]
        public async Task<ActionResult<int>> GetStudentCountByCenterId(long id)
        {
            var result = await _studentRepository.GetCenterStudentsCountAsync(id);
            return Ok(result);
        }



        // ========================================
        //  GET: api/Student/paginated
        //  Server-Side Filtering + Pagination
        // ========================================
        [HttpGet("paginated")]
        public async Task<ActionResult<PaginatedResponse<Student>>>
            GetPaginated([FromQuery] StudentFilterRequest request)
        {
            var response = await _studentRepository.GetPaginatedStudentsAsync(request);
            return Ok(response);
        }

    }
}