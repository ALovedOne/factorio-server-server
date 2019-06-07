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
        public Server Get(string slug)
        {
            return _servers.getById(slug);
        }

        // POST: api/Server
        [HttpPost]
        public string Post([FromBody] Server value)
        {
            return _servers.addServer(value);
        }

        // PUT: api/Server/5
        [HttpPut("{slug}")]
        public void Put(string slug, [FromBody] Server value)
        {
            _servers.updateServer(slug, value);
        }
    }
}
