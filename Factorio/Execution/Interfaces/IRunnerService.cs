using Factorio.Persistence.Interfaces;
using Factorio.Persistence.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Factorio.Execution
{
    public interface IRunnerService
    {
        Task<IEnumerable<IRunningInstance>> getRunningInstancesAsync();
        // Get by id
        Task<IRunningInstance> startInstanceAsync(string host, int port, IInstance instance);

        Task stopInstanceAsync(string host, int port);
        Task stopInstanceAsync(string key);
    }
}
