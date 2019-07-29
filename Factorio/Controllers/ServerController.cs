using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using factorio.Models;
using factorio.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace factorio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private IServerProvider _servers;

        public ServerController(IServerProvider servers)
        {
            _servers = servers;
        }

        // GET: api/Server
        [HttpGet]
        public IEnumerable<Server> Get()
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
        public IActionResult Post([FromBody] Server value)
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
        public void Put(string slug, [FromBody] Server value)
        {
            _servers.updateServer(slug, value);
        }
    }
}
