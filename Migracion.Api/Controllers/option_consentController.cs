using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Service.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class option_consentController : ControllerBase
    {
        private readonly IOptionConsentQueries _optionConsentQueries;

        public option_consentController(IOptionConsentQueries optionConsentQueries)
        {
            _optionConsentQueries = optionConsentQueries;
        }

        [HttpPost]
        [Route("Insert")]
        public TResult Insert([FromBody] option_consent item)
        {
            return _optionConsentQueries.Insert(item);
        }

        [HttpGet]
        [Route("GetByReg/{cmpy}/{reg}")]
        public async Task<List<option_consent>> GetByReg(string cmpy, string reg)
        {
            return await _optionConsentQueries.GetByReg(cmpy, reg);
        }

        [HttpGet]
        [Route("GetByCelular/{cmpy}/{celular}")]
        public async Task<List<option_consent>> GetByCelular(string cmpy, string celular)
        {
            return await _optionConsentQueries.GetByCelular(cmpy, celular);
        }
    }
}
