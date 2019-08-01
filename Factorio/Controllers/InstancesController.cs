using Factorio.Models;
using Factorio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Factorio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstancesController : ControllerBase
    {
        private IInstanceProvider _servers;

        public InstancesController(IInstanceProvider servers)
        {
            _servers = servers;
        }

        // GET: api/Server
        [HttpGet]
        public IEnumerable<GameInstance> Get()
        {
            return _servers.GetAll();
        }

        // GET: api/Server/5
        [HttpGet("{slug}")]
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
        public IActionResult Post([FromBody] GameInstance value)
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
        public void Put(string slug, [FromBody] GameInstance value)
        {
            _servers.UpdateServer(slug, value);
        }
    }
}
