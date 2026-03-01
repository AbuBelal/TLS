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
    public class StdCenterController : ControllerBase
    {
        private readonly IStdCenterRepository _stdCenterRepository;

        public StdCenterController(IStdCenterRepository stdCenterRepository)
        {
            _stdCenterRepository = stdCenterRepository;
        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<ActionResult<List<StdCenter>>> GetAll()
        {
            var result = await _stdCenterRepository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StdCenter>> GetById(long id)
        {
            var result = await _stdCenterRepository.GetById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Insert(StdCenter stdCenter)
        {
            var response = await _stdCenterRepository.Insert(stdCenter);
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<GeneralResponse>> Update(StdCenter stdCenter)
        {
            var response = await _stdCenterRepository.Update(stdCenter);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> Delete(long id)
        {
            var response = await _stdCenterRepository.DeleteById(id);
            return Ok(response);
        }
    }
}