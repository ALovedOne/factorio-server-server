using Factorio.Models;
using Factorio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Factorio.Controllers;
using Xunit;

namespace Factorio.Test
{
    public class TestInstanceController
    {
        [Fact]
        public async Task TestThings()
        {
            IInstanceProvider instanceProvider = new MockInstanceProvider(new GameInstance[0]);
            IRunnerService executionProvider = new MockExecutionProvider(new ExecutionInfo[0]);
            InstancesController controller = new InstancesController(instanceProvider, executionProvider);

            IEnumerable<GameInstance> result = await controller.Get();
        }

    }

    class MockInstanceProvider : IInstanceProvider
    {
        private readonly IEnumerable<GameInstance> _data;

        public MockInstanceProvider(IEnumerable<GameInstance> mockData)
        {
            _data = mockData;
        }

        public IEnumerable<GameInstance> GetAll()
        {
            return _data;
        }

        public GameInstance GetById(string key)
        {
            return _data.FirstOrDefault(e => e.Key == key);
        }

        public bool IdExists(string key)
        {
            return GetById(key) != null;
        }

        public GameInstance TryAddServer(GameInstance value)
        {
            throw new NotImplementedException();
        }

        public GameInstance UpdateServer(GameInstance value)
        {
            throw new NotImplementedException();
        }
    }

    class MockExecutionProvider : IRunnerService
    {
        private readonly IEnumerable<ExecutionInfo> _data;

        public MockExecutionProvider(IEnumerable<ExecutionInfo> mockData)
        {
            _data = mockData;
        }

        public async Task<ExecutionInfo> GetByExecutionInfoKeyAsync(string key)
        {
            return _data.FirstOrDefault(r => r.InstanceKey == key);
        }

        public async Task<IEnumerable<ExecutionInfo>> GetExecutionInfosAsync()
        {
            return _data;
        }

        public Task<ExecutionInfo> StartInstanceAsync(string host, int port, GameInstance instance)
        {
            throw new NotImplementedException();
        }

        public Task StopInstanceAsync(string host, int port)
        {
            throw new NotImplementedException();
        }

        public Task StopInstanceAsync(string key)
        {
            throw new NotImplementedException();
        }
    }
}
