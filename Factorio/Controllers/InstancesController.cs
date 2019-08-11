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
        public async Task<IEnumerable<GameInstance>> Get()
        {
            Task<IEnumerable<ExecutionInfo>> ExecutionInfosTask = _runner.GetExecutionInfosAsync();

            IEnumerable<GameInstance> gameInstances = _servers.GetAll();

            IEnumerable<ExecutionInfo> ExecutionInfos = await ExecutionInfosTask;

            return gameInstances.Select(g => { g.CurrentExecution = ExecutionInfos.SingleOrDefault(r => r.InstanceKey == g.Key); return g; });
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetAsync(string key)
        {
            GameInstance g = _servers.GetById(key);
            if (g == null)
                return NotFound(key);

            g.CurrentExecution = await _runner.GetByExecutionInfoKeyAsync(key);
            return Ok(g);
        }

        [HttpPost]
        public IActionResult Post([FromBody] GameInstance value)
        {
            GameInstance newInstance = _servers.TryAddServer(value);
            if (newInstance != null)
                return Ok(newInstance);
            return BadRequest();
        }

        [HttpPut("{key}")]
        public IActionResult Put(string key, [FromBody] GameInstance value)
        {
            GameInstance updateInstance = _servers.UpdateServer(value);
            if (updateInstance == null)
                return BadRequest("TODO - something went wrong");
            return Ok(updateInstance);
        }


        [HttpPost("{key}/start")]
        public async Task<IActionResult> StartInstance(string key, int port)
        {
            ExecutionInfo r = await _runner.GetByExecutionInfoKeyAsync(key);
            if (r != null) return BadRequest("Instance is alread running");


            GameInstance instance = _servers.GetById(key);
            if (instance == null) return NotFound(key);

            r = await _runner.StartInstanceAsync("localhost", port, instance);
            if (r == null) BadRequest();

            instance.CurrentExecution = r;
            return Ok(instance);
        }

        [HttpPost("{key}/restart")]
        public async Task<IActionResult> RestartInstace(string key)
        {
            ExecutionInfo r = await _runner.GetByExecutionInfoKeyAsync(key);
            if (r == null) return BadRequest("Instance is not running");

            GameInstance instance = _servers.GetById(key);
            if (instance == null) NotFound(key);

            await _runner.StopInstanceAsync(key);
            r = await _runner.StartInstanceAsync(r.Hostname, r.Port, instance);
            if (r == null) return BadRequest();

            instance.CurrentExecution = r;
            return Ok(instance);
        }

        [HttpPost("{key}/stop")]
        public async Task<IActionResult> StopInstance(string key)
        {
            ExecutionInfo r = await _runner.GetByExecutionInfoKeyAsync(key);
            if (r == null) return BadRequest("Instance is not running");

            await _runner.StopInstanceAsync(key);
            GameInstance g = _servers.GetById(key);
            g.CurrentExecution = null;
            return Ok(g);
        }

        private IActionResult NotFound(string key)
        {
            return base.NotFound(string.Format("Instance identified by {0} not found", key));
        }
    }
}
