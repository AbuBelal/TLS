using SharedLib.Entities;
using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Responses;

namespace APIServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class CenterController : ControllerBase
    {
        private readonly ICenterRepository _centerRepository;

        public CenterController(ICenterRepository centerRepository)
        {
            _centerRepository = centerRepository;
        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<ActionResult<List<Center>>> GetAll()
        {
            var result = await _centerRepository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Center>> GetById(long id)
        {
            var result = await _centerRepository.GetById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Insert(Center center)
        {
            var response = await _centerRepository.Insert(center);
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<GeneralResponse>> Update(Center center)
        {
            var response = await _centerRepository.Update(center);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> Delete(long id)
        {
            var response = await _centerRepository.DeleteById(id);
            return Ok(response);
        }
    }
}