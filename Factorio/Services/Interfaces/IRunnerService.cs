using Factorio.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Factorio.Services.Interfaces
{
    public interface IRunnerService
    {
        Task<IEnumerable<ExecutionInfo>> GetExecutionInfosAsync();
        Task<ExecutionInfo> GetByExecutionInfoKeyAsync(string key);

        Task<ExecutionInfo> StartInstanceAsync(string host, int port, GameInstance instance);

        Task StopInstanceAsync(string host, int port);
        Task StopInstanceAsync(string key);
        
    }
}
