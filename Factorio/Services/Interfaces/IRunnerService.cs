using Factorio.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Factorio.Services.Interfaces
{
    public interface IRunnerService
    {
        Task<IEnumerable<RunningInstance>> GetRunningInstancesAsync();
        // Get by id
        Task<RunningInstance> StartInstanceAsync(string host, int port, GameInstance instance);

        Task StopInstanceAsync(string host, int port);
        Task StopInstanceAsync(string key);
    }
}
