using Factorio.Persistence.Interfaces;
using Factorio.Persistence.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Factorio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private IInstanceProvider _servers;

        public ServerController(IInstanceProvider servers)
        {
            _servers = servers;
        }

        // GET: api/Server
        [HttpGet]
        public IEnumerable<IInstance> Get()
        {
            return _servers.GetAll();
        }

        // GET: api/Server/5
        [HttpGet("{slug}", Name = "Get")]
        public IActionResult Get(string slug)
        {
            if (_servers.IdExists(slug))
            {
                return Ok(_servers.GetById(slug));
            }
            else
            {
                return NotFound();
            }
        }

        // POST: api/Server
        [HttpPost]
        public IActionResult Post([FromBody] Instance value)
        {
            if (_servers.TryAddServer(value, out string newId))
            {
                return Ok(newId);
            }
            else
            {
                return this.BadRequest();
            }
        }

        // PUT: api/Server/5
        [HttpPut("{slug}")]
        public void Put(string slug, [FromBody] Instance value)
        {
            _servers.UpdateServer(slug, value);
        }
    }
}
