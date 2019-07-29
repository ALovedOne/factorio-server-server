using Factorio.Persistence;
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
            return _servers.getAll();
        }

        // GET: api/Server/5
        [HttpGet("{slug}", Name = "Get")]
        public IActionResult Get(string slug)
        {
            if (_servers.idExists(slug))
            {
                return Ok(_servers.getById(slug));
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
            string newId;

            if (_servers.tryAddServer(value, out newId))
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
            _servers.updateServer(slug, value);
        }
    }
}
