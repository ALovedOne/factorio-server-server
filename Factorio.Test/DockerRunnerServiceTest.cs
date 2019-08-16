using Docker.DotNet;
using Docker.DotNet.Models;
using Factorio.Models;
using Factorio.Services.Execution.DockerImplementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;


namespace Factorio.Test
{
    public class DockerRunnerServiceTest : TestWithInstanceProvider, IDisposable
    {
        private readonly DockerClient _dockerClient;
        private readonly DockerRunnerService _service;

        public DockerRunnerServiceTest() : base()
        {
            IOptions<DockerExecutionOptions> options = Options.Create<DockerExecutionOptions>(new DockerExecutionOptions
            {
                DockerUrl = "npipe://./pipe/docker_engine",
                PortRangeBegin = 9990,
                PortRangeEnd = 10000
            });

            ILogger<DockerRunnerService> logger = new Mock<ILogger<DockerRunnerService>>().Object;
            _dockerClient = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")) // TODO - handle windows/linux
                    .CreateClient();

            _service = new DockerRunnerService(options, logger);

        }

        public new void Dispose()
        {
            Task killingContainers = KillAllDockerInstancesAsync();
            killingContainers.Wait();
            base.Dispose();
        }

        [Fact]
        public void TestMountWithLocalFile()
        {
            string gameKey = "test-game-key";
            Uri configUrl = new Uri("file:///localpath/save/path/");

            VolumesCreateParameters volCreate = DockerRunnerService.CreateVolumeRequest(gameKey, configUrl);
            Assert.Null(volCreate);

            Mount m = DockerRunnerService.ConfigUrlToMount(gameKey, configUrl);

            Assert.NotNull(m);
            Assert.Equal("/factorio", m.Target);
            Assert.Equal("bind", m.Type);
            Assert.Equal("/localpath/save/path/", m.Source);
        }

        [Fact]
        public void TestMountWithNFSFile()
        {
            string gameKey = "test-game-key";
            Uri configUrl = new Uri("nfs://nfs-server/path/to/save");

            VolumesCreateParameters volCreate = DockerRunnerService.CreateVolumeRequest(gameKey, configUrl);
            Assert.NotNull(volCreate);
            Assert.Equal("local", volCreate.Driver);
            Assert.Equal("factorio-volume-test-game-key", volCreate.Name);
            AssertDictContains(volCreate.DriverOpts, "device", "/path/to/save");
            AssertDictContains(volCreate.DriverOpts, "type", "nfs");
            AssertDictContains(volCreate.DriverOpts, "o", "addr=nfs-server,vers=4,soft");

            VolumeResponse volumeResponse = new VolumeResponse
            {
                Name = "docker-volume"
            };

            Mount m = DockerRunnerService.ConfigUrlToMount(gameKey, configUrl);

            Assert.NotNull(m);
            Assert.Equal("/factorio", m.Target);
        }

        private void AssertDictContains(IDictionary<string, string> dict, string key, string value)
        {
            Assert.True(dict.TryGetValue(key, out string dictValue));
            Assert.Equal(value, dictValue);
        }

        [Fact]
        public async Task TestStartInstanceAsync()
        {
            AddBlankDir("blank-dir");

            GameInstance i = GetGameInstance("blank-dir");

            ExecutionInfo x = await this._service.StartInstanceAsync("localhost", 9999, i);
            ExecutionInfo dockerInst = x as ExecutionInfo;

            ContainerInspectResponse containerInfo = await _dockerClient.Containers.InspectContainerAsync(dockerInst.Key);
            Assert.Equal("factoriotools/factorio:0.17", containerInfo.Config.Image);

            IList<PortBinding> udpPorts = containerInfo.NetworkSettings.Ports["34197/udp"];
            Assert.Single(udpPorts);
            Assert.Equal("9999", udpPorts[0].HostPort);

            Assert.Single(containerInfo.Mounts);
            Assert.Equal("/factorio", containerInfo.Mounts[0].Destination);
            Assert.Equal("bind", containerInfo.Mounts[0].Type);
            //Assert.Equal(i.LocalPath, containerInfo.Mounts[0].Source); // On windows this is a weird translation
        }

        [Fact]
        public async Task TestStartInstance2Async()
        {
            AddBlankDir("blank-dir", 54);
            GameInstance i = GetGameInstance("blank-dir");

            ExecutionInfo dockerInst = await _service.StartInstanceAsync("localhost", 9999, i);

            ContainerInspectResponse containerInfo = await _dockerClient.Containers.InspectContainerAsync(dockerInst.Key);
            Assert.Equal("factoriotools/factorio:0.17", containerInfo.Config.Image);

            IList<PortBinding> udpPorts = containerInfo.NetworkSettings.Ports["34197/udp"];
            Assert.Single(udpPorts);
            Assert.Equal("9999", udpPorts[0].HostPort);

            Assert.Single(containerInfo.Mounts);
            Assert.Equal("/factorio", containerInfo.Mounts[0].Destination);
            Assert.Equal("bind", containerInfo.Mounts[0].Type);
            //Assert.Equal(i.LocalPath, containerInfo.Mounts[0].Source); // On windows this is a weird translation
        }
            
        /*
        [Fact]
        public async Task TestStartingMultipleWithSameIDAsync()
        {
            // TODO - should catch this case
        }
        */

        [Fact]
        public async Task TestGetExecutionInfosAsync()
        {
            AddTestSave("save_15_40");
            GameInstance i1 = GetGameInstance("save_15_40");

            AddTestSave("save_17_50");
            GameInstance i2 = GetGameInstance("save_17_50");

            await _service.StartInstanceAsync("localhost", 9999, i1);
            await _service.StartInstanceAsync("localhost", 9998, i2);

            IList<ExecutionInfo> ExecutionInfos = new List<ExecutionInfo>(await _service.GetExecutionInfosAsync());

            Assert.Equal(2, ExecutionInfos.Count);
        }

        private async Task KillAllDockerInstancesAsync()
        {
            IList<ContainerListResponse> runningContainers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true,
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    {"label", new Dictionary<string, bool>
                    {
                        {"factorio-server-key", true }
                    } }
                }
            });

            foreach (ContainerListResponse r in runningContainers)
            {
                await _dockerClient.Containers.RemoveContainerAsync(r.ID, new ContainerRemoveParameters
                {
                    RemoveVolumes = true,
                    Force = true
                });
            }

        }
    }
}
