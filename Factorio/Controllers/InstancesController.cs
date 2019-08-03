using Factorio.Models;
using Factorio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Factorio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstancesController : ControllerBase
    {
        private readonly IInstanceProvider _servers;
        private readonly IRunnerService _runner;

        public InstancesController(IInstanceProvider servers, IRunnerService runner)
        {
            _servers = servers;
            _runner = runner;
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


        [HttpGet("{key}/thumbnail")]
        public string Thumbnail(string key)
        {
            return "Pong";
        }

        [HttpGet("{key}/restart")]
        public async Task<IActionResult> Start(string key)
        {
            RunningInstance runningInstance = await this._runner.GetByInstanceKeyAsync(key);
            if (runningInstance == null) return NotFound();


            return BadRequest("TODO");
        }
    }
}
