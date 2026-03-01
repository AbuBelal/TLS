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
    public class LookupValueController : ControllerBase
    {
        private readonly ILookupValueRepository _lookupValueRepository;

        public LookupValueController(ILookupValueRepository lookupValueRepository)
        {
            _lookupValueRepository = lookupValueRepository;
        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<ActionResult<List<LookupValue>>> GetAll()
        {
            var result = await _lookupValueRepository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LookupValue>> GetById(long id)
        {
            var result = await _lookupValueRepository.GetById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Insert(LookupValue lookupValue)
        {
            var response = await _lookupValueRepository.Insert(lookupValue);
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<GeneralResponse>> Update(LookupValue lookupValue)
        {
            var response = await _lookupValueRepository.Update(lookupValue);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> Delete(long id)
        {
            var response = await _lookupValueRepository.DeleteById(id);
            return Ok(response);
        }
    }
}