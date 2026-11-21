using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migracion.Models;
using Migracion.Models.Dtos;
using Migracion.Persistence.DataBase;
using Migracion.Service.Queries;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ListasPreciosController : ControllerBase
    {
        private readonly IListasPreciosQueries _listaPreciosQueries;

        public ListasPreciosController(IListasPreciosQueries listaPreciosQueries)
        {
            _listaPreciosQueries = listaPreciosQueries;
        }

        [HttpGet]
        [Route("GetListByCodUsu/{cmpy}/{codUsu}")]
        public async Task<List<mob_ritempricDto>> GetListByCodUsu(string cmpy, string codUsu)
        {
            return await _listaPreciosQueries.GetListByCodUsu(cmpy, codUsu);
            
        }

        [HttpGet]
        [Route("GetListProducts/{cmpy}/{lista}/{codUsu}")]
        public async Task<List<mob_ritempricE>> GetListProducts(string cmpy,string lista, string codUsu)
        {
            return await _listaPreciosQueries.GetListProducts(cmpy, lista, codUsu); 
        }
    }
}
