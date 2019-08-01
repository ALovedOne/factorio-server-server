using Factorio.Models;
using Factorio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Factorio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExecutionController : ControllerBase
    {
        private readonly IInstanceProvider _instances;
        private readonly IRunnerService _runner;

        public ExecutionController(IRunnerService runner, IInstanceProvider instanceService)
        {
            this._runner = runner;
        }

        // GET: api/Execution
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _runner.GetRunningInstancesAsync());
        }

        // GET: api/Execution/5
        [HttpGet("{key}", Name = "getExecutionById")]
        public async Task<IActionResult> GetAsync(string key)
        {
            IEnumerable<RunningInstance> instances = await this._runner.GetRunningInstancesAsync();

            RunningInstance instance = instances.FirstOrDefault(x => x.Key == key);
            if (instance != null)
            {
                return Ok(instance);
            }

            return NotFound();
        }

        // POST: api/Execution
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RunningInstance value)
        {
            RunningInstance ret = null;

            GameInstance gameInstance = this._instances.GetById(value.InstanceKey);
            if (gameInstance == null)
                return NotFound(string.Format("Game instance idenditifed by \"{0}\" not found", value.InstanceKey));

            ret = await this._runner.StartInstanceAsync("", value.Port, gameInstance);
            return Created(Url.RouteUrl("getExecutionById", ret), ret);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string Key)
        {
            IEnumerable<RunningInstance> instances = await this._runner.GetRunningInstancesAsync();
            RunningInstance instance = instances.FirstOrDefault(x => x.Key == Key);
            if (instance == null)
            {
                return NotFound();
            }

            await this._runner.StopInstanceAsync(Key);
            return Ok("");
        }
    }
}
