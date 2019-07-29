using Docker.DotNet;
using Docker.DotNet.Models;
using Factorio.Execution;
using Factorio.Persistence.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;


namespace Factorio.Test
{
    public class DockerRunnerServiceTest : IDisposable
    {
        private DockerClient _dockerClient;
        private DockerRunnerService _service;

        private DirectoryInfo _testDir;


        public DockerRunnerServiceTest()
        {

            DirectoryInfo userTempDir = new DirectoryInfo(Path.GetTempPath());
            string randomName = Path.GetRandomFileName();
            this._testDir = userTempDir.CreateSubdirectory(randomName);

            this._dockerClient = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")) // TODO - handle windows/linux
                    .CreateClient();

            IConfigurationRoot configRoot = new ConfigurationBuilder()
                //.AddInMemoryCollection(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("LocalPersistenceProvider:BaseDirectory", this._testDir.FullName) })
                .Build();
            this._service = new DockerRunnerService(configRoot);
        }

        public void Dispose()
        {
            Task killingContainers = this.killAllDockerInstancesAsync();
            killingContainers.Wait();

            if (this._testDir.Exists)
            {
                this._testDir.Delete(true);
            }
        }

        [Fact]
        public async Task TestStartInstanceAsync()
        {
            IRunningInstance x = await this._service.startInstanceAsync("localhost", 9999, new Instance
            {
                Slug = "some-slug",
                TargetMajorVersion = 17,
                TargetMinorVersion = 54,
            });
            DockerRunningInstance dockerInst = x as DockerRunningInstance;


            ContainerInspectResponse containerInfo = await this._dockerClient.Containers.InspectContainerAsync(dockerInst.ContainerID);
            Assert.Equal("factoriotools/factorio:0.17.54", containerInfo.Config.Image);

            IList<PortBinding> udpPorts = containerInfo.NetworkSettings.Ports["34197/udp"];
            Assert.Single(udpPorts);
            Assert.Equal("9999", udpPorts[0].HostPort);

            Assert.Single(containerInfo.Mounts);
            Assert.Equal("/factorio", containerInfo.Mounts[0].Destination);
            Assert.Equal("bind", containerInfo.Mounts[0].Driver);
            Assert.Equal("TODO", containerInfo.Mounts[0].Source);

            //containerInfo.Mounts
        }


        private async Task killAllDockerInstancesAsync()
        {
            IList<ContainerListResponse> runningContainers = await this._dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {

                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    {"label", new Dictionary<string, bool>
                    {
                        {"factorio-server=yes", true }
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
