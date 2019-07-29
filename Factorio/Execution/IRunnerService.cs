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
        // stop
    }
}
