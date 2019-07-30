using Factorio.Persistence.Interfaces;
using Factorio.Persistence.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Factorio.Execution
{
    public interface IRunnerService
    {
        Task<IEnumerable<IRunningInstance>> GetRunningInstancesAsync();
        // Get by id
        Task<IRunningInstance> StartInstanceAsync(string host, int port, IInstance instance);

        Task StopInstanceAsync(string host, int port);
        Task StopInstanceAsync(string key);
    }
}
