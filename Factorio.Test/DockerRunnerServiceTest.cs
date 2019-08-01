using Docker.DotNet;
using Docker.DotNet.Models;
using Factorio.Models;
using Factorio.Services.Execution.DockerImplementation;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;


namespace Factorio.Test
{
    public class DockerRunnerServiceTest : TestWithStuff, IDisposable
    {
        private readonly DockerClient _dockerClient;
        private readonly DockerRunnerService _service;

        public DockerRunnerServiceTest() : base()
        {

            this._dockerClient = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")) // TODO - handle windows/linux
                    .CreateClient();

            IConfigurationRoot configRoot = new ConfigurationBuilder()
                //.AddInMemoryCollection(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("LocalPersistenceProvider:BaseDirectory", this._testDir.FullName) })
                .Build();
            this._service = new DockerRunnerService(configRoot);
        }

        public new void Dispose()
        {
            Task killingContainers = this.KillAllDockerInstancesAsync();
            killingContainers.Wait();
            base.Dispose();
        }

        [Fact]
        public async Task TestStartInstanceAsync()
        {
            GameInstance i = this.AddBlankDir("blank-dir");

            RunningInstance x = await this._service.StartInstanceAsync("localhost", 9999, i);
            RunningInstance dockerInst = x as RunningInstance;

            ContainerInspectResponse containerInfo = await this._dockerClient.Containers.InspectContainerAsync(dockerInst.Key);
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
            GameInstance i = this.AddBlankDir("blank-dir", 54);

            RunningInstance dockerInst = await this._service.StartInstanceAsync("localhost", 9999, i);

            ContainerInspectResponse containerInfo = await this._dockerClient.Containers.InspectContainerAsync(dockerInst.Key);
            Assert.Equal("factoriotools/factorio:0.17.54", containerInfo.Config.Image);

            IList<PortBinding> udpPorts = containerInfo.NetworkSettings.Ports["34197/udp"];
            Assert.Single(udpPorts);
            Assert.Equal("9999", udpPorts[0].HostPort);

            Assert.Single(containerInfo.Mounts);
            Assert.Equal("/factorio", containerInfo.Mounts[0].Destination);
            Assert.Equal("bind", containerInfo.Mounts[0].Type);
            //Assert.Equal(i.LocalPath, containerInfo.Mounts[0].Source); // On windows this is a weird translation
        }

        [Fact]
        public async Task TestStartingMultipleWithSameIDAsync()
        {
            // TODO - should catch this case
        }

        [Fact]
        public async Task TestGetRunningInstancesAsync()
        {
            GameInstance i1 = this.AddTestSave("save_15_40");
            GameInstance i2 = this.AddTestSave("save_17_50");

            await this._service.StartInstanceAsync("localhost", 9999, i1);
            await this._service.StartInstanceAsync("localhost", 9998, i2);

            IList<RunningInstance> runningInstances = new List<RunningInstance>(await this._service.GetRunningInstancesAsync());

            Assert.Equal(2, runningInstances.Count);
        }

        private async Task KillAllDockerInstancesAsync()
        {
            IList<ContainerListResponse> runningContainers = await this._dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {

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
                await this._dockerClient.Containers.RemoveContainerAsync(r.ID, new ContainerRemoveParameters
                {
                    RemoveVolumes = true,
                    Force = true
                });
            }

        }
    }
}
