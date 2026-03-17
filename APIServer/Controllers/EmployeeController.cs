using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Responses;
using SharedLib.Entities;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<ActionResult<List<Employee>>> GetAll()
        {
            var result = await _employeeRepository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetById(long id)
        {
            var result = await _employeeRepository.GetById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Insert(Employee employee)
        {
            var response = await _employeeRepository.Insert(employee);
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<GeneralResponse>> Update(Employee employee)
        {
            var response = await _employeeRepository.Update(employee);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> Delete(long id)
        {
            var response = await _employeeRepository.DeleteById(id);
            return Ok(response);
        }
    }
}