using Docker.DotNet;
using Docker.DotNet.Models;
using Factorio.Persistence.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Factorio.Execution
{
    public class DockerRunnerService : IRunnerService
    {
        private const string CONFIG_SECTION_NAME = "DockerRunner";
        private const string CONFIG_CONNECTION_URL = "Uri";

        private DockerClient _dockerClient;

        public DockerRunnerService(IConfiguration config)
        {
            string defaultUrl = "npipe://./pipe/docker_engine";
            // string defaultUrl = "unix:/var/run/docker.sock";

            IConfigurationSection section = config.GetSection(CONFIG_SECTION_NAME);
            string dockerConnectionUri = section.GetValue<string>(CONFIG_CONNECTION_URL, defaultUrl);

            this._dockerClient = new DockerClientConfiguration(new Uri(dockerConnectionUri)).CreateClient();
        }

        public async Task<IEnumerable<IRunningInstance>> getRunningInstancesAsync()
        {

            IList<ContainerListResponse> x = await this._dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {

                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    {"label", new Dictionary<string, bool>
                    {
                        {"factorio-server=yes", true }
                    } }
                }
            });

            return new DockerRunningInstance[0];
        }


        public async Task<IRunningInstance> startInstanceAsync(string host, int port, IInstance instance)
        {
            await this.loadImageAsync(instance);
            string newID = await this.startImageAsync(instance, port);

            DockerRunningInstance i = new DockerRunningInstance
            {
                Hostname = host,
                Port = port,

                Version = formatVersion(instance),
                InstanceKey = instance.Slug,

                ContainerID = newID,
            };
            return i;
        }


        private async Task loadImageAsync(IInstance instance)
        {

            await this._dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = "factoriotools/factorio",
                Tag = formatVersion(instance)
            }, new AuthConfig(), new ProgressSink());

        }

        private async Task<string> startImageAsync(IInstance instance, int port)
        {
            CreateContainerResponse newContainer = await this._dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Labels = new Dictionary<string, string>
                {
                    {"factorio-server", "yes" },
                    {"factorio-server-key", instance.Slug }
                },
                Volumes = new Dictionary<string, EmptyStruct> { { "/factorio", new EmptyStruct() } },
                Image = "factoriotools/factorio:" + formatVersion(instance),
                ExposedPorts = new Dictionary<string, EmptyStruct> { { "34197/udp", new EmptyStruct() } },
                Name = "factorio-server-" + instance.Slug,
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>> { { "34197/udp", new[] { new PortBinding { HostPort = string.Format("{0}", port) } } } },
                    Mounts = new List<Mount>
                    {
                        new Mount
                        {
                            Target = "/factorio",
                            Type="bind",
                            Source = @"C:\Users\mike\Desktop\opt\factorio1" // TODO - get actual bind path
                        }
                    }
                }
            });

            await this._dockerClient.Containers.StartContainerAsync(newContainer.ID, new ContainerStartParameters());

            return newContainer.ID;
        }

        private string formatVersion(IInstance instance)
        {
            string version = string.Format("0.{0}", instance.TargetMajorVersion);
            if (instance.TargetMinorVersion != null)
            {
                version = string.Format("{0}.{1}", version, instance.TargetMinorVersion);
            }

            return version;
        }
    }

    internal class ProgressSink : IProgress<JSONMessage>
    {
        public void Report(JSONMessage value)
        {
        }
    }
}
