using Factorio.Models;
using Factorio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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
        public async Task<IEnumerable<ReturnDict>> Get()
        {
            Task<IEnumerable<RunningInstance>> runningInstancesTask = _runner.GetRunningInstancesAsync();

            IEnumerable<GameInstance> gameInstances = _servers.GetAll();

            IEnumerable<RunningInstance> runningInstances = await runningInstancesTask;

            return gameInstances.Select(g => new ReturnDict
            {
                Key = g.Key,
                Save = g,
                Execution = runningInstances.SingleOrDefault(r => r.InstanceKey == g.Key)
            });
        }

        // GET: api/Server/5
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetAsync(string slug)
        {
            if (_servers.IdExists(slug))
            {
                ReturnDict ret = new ReturnDict
                {
                    Key = slug,
                    Save = _servers.GetById(slug),
                    Execution = await _runner.GetByRunningInstanceKeyAsync(slug)
                };
                return Ok(ret);
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


        [HttpPost("{key}/start")]
        public async Task<IActionResult> StartInstance(string key, int port)
        {
            RunningInstance r = await this._runner.GetByRunningInstanceKeyAsync(key);
            if (r != null) return BadRequest("Key is alread running");


            GameInstance instance = _servers.GetById(key);
            if (instance == null) return NotFound(""); // TODO - description

            r = await _runner.StartInstanceAsync("localhost", port, instance);
            return Ok(r);
        }

        [HttpPost("{key}/restart")]
        public async Task<IActionResult> RestartInstace(string key)
        {
            RunningInstance r = await this._runner.GetByRunningInstanceKeyAsync(key);
            if (r == null) return BadRequest("Instance is not running");

            GameInstance instance = _servers.GetById(key);
            if (instance == null) return NotFound(""); // TODO - description

            await _runner.StopInstanceAsync(key);
            r = await _runner.StartInstanceAsync(r.Hostname, r.Port, instance);
            return Ok(new ReturnDict
            {
                Key = key,
                Save = instance,
                Execution = r
            });
        }

        [HttpPost("{key}/stop")]
        public async Task<IActionResult> StopInstance(string key)
        {
            RunningInstance r = await this._runner.GetByRunningInstanceKeyAsync(key);
            if (r == null) return BadRequest("Instance is not running");

            await _runner.StopInstanceAsync(key);
            return Ok(new ReturnDict
            {
                Key = key,
                Save = _servers.GetById(key),
                Execution = null
            });
        }
    }

    public class ReturnDict
    {
        public string Key { get; set; }
        public GameInstance Save { get; set; }
        public RunningInstance Execution { get; set; }
    }
}
