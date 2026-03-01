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
    public class EmpCenterController : ControllerBase
    {
        private readonly IEmpCenterRepository _empCenterRepository;

        public EmpCenterController(IEmpCenterRepository empCenterRepository)
        {
            _empCenterRepository = empCenterRepository;
        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<ActionResult<List<EmpCenter>>> GetAll()
        {
            var result = await _empCenterRepository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmpCenter>> GetById(long id)
        {
            var result = await _empCenterRepository.GetById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Insert(EmpCenter empCenter)
        {
            var response = await _empCenterRepository.Insert(empCenter);
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<GeneralResponse>> Update(EmpCenter empCenter)
        {
            var response = await _empCenterRepository.Update(empCenter);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> Delete(long id)
        {
            var response = await _empCenterRepository.DeleteById(id);
            return Ok(response);
        }
    }
}