using Docker.DotNet;
using Docker.DotNet.Models;
using Factorio.Models;
using Factorio.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Factorio.Services.Execution.DockerImplementation
{
    public class DockerRunnerService : IRunnerService
    {
        private const string CONFIG_SECTION_NAME = "DockerRunner";
        private const string CONFIG_CONNECTION_URL = "Uri";

        private const string DOCKER_LABEL_KEY = "factorio-server-key";

        private readonly IDictionary<string, string> _imageVersionMap = new Dictionary<string, string>();
        private readonly DockerClient _dockerClient;

        public DockerRunnerService(IConfiguration config)
        {
            // TODO - handle figuring which type of machine you are on
            string defaultUrl = "npipe://./pipe/docker_engine";
            // string defaultUrl = "unix:/var/run/docker.sock";

            IConfigurationSection section = config.GetSection(CONFIG_SECTION_NAME);
            string dockerConnectionUri = section.GetValue<string>(CONFIG_CONNECTION_URL, defaultUrl);

            this._dockerClient = new DockerClientConfiguration(new Uri(dockerConnectionUri)).CreateClient();
        }

        public async Task<IEnumerable<RunningInstance>> GetRunningInstancesAsync()
        {
            IEnumerable<ContainerListResponse> runningInstances = await this._dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {

                Filters = new Dictionary<string, IDictionary<string, bool>> { { "label", new Dictionary<string, bool> { { DOCKER_LABEL_KEY, true } } } }
            });

            Dictionary<string, string> versionMap = new Dictionary<string, string>();
            foreach (ContainerListResponse containerInfo in runningInstances)
            {
                string version = await this.GetImageVersionAsync(containerInfo.ImageID);
                versionMap.TryAdd(containerInfo.ImageID, version);
            }

            return runningInstances.Select(containerInfo =>
            {
                return new RunningInstance
                {
                    Key = containerInfo.ID,
                    Hostname = "localhost",
                    Port = containerInfo.Ports.First(p => p.PrivatePort == 34197).PublicPort,
                    RunningVersion = versionMap[containerInfo.ImageID],
                    InstanceKey = containerInfo.Labels[DOCKER_LABEL_KEY]
                };
            });
        }

        public async Task<RunningInstance> StartInstanceAsync(string host, int port, GameInstance instance)
        {
            await this.LoadImageAsync(instance);
            // TODO - handle invliad info
            string newID = await this.StartImageAsync(instance, port, instance.ImplementationInfo.GetValueOrDefault("localPath", ""));

            RunningInstance i = new RunningInstance
            {
                Key = newID,
                Hostname = host,
                Port = port,

                RunningVersion = FormatVersion(instance),
                InstanceKey = instance.Key,
            };
            return i;
        }

        public async Task StopInstanceAsync(string host, int port)
        {
            string containerID = await this.GetContainerIDAsync(port: port);
            await this.StopContainerByIDAsync(containerID);
        }

        public async Task StopInstanceAsync(string key)
        {
            string containerID = await this.GetContainerIDAsync(key: key);
            await this.StopContainerByIDAsync(containerID);
        }

        private async Task<string> GetContainerIDAsync(int? port = null, string key = null)
        {
            IDictionary<string, IDictionary<string, bool>> filters = new Dictionary<string, IDictionary<string, bool>>();

            if (port != null)
            {
                filters.Add("publish", new Dictionary<string, bool> { { string.Format("{0}", port), true } });
            }

            if (key != null)
            {
                filters.Add("label", new Dictionary<string, bool> { { string.Format("key={0}", key), true } });
            }

            IList<ContainerListResponse> containers = await this._dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                Filters = filters
            });

            if (containers.Count < 1)
            {
                return "";
            }
            ContainerListResponse first = containers[0];
            return first.ID;
        }


        private async Task StopContainerByIDAsync(string containerID)
        {
            await this._dockerClient.Containers.RemoveContainerAsync(containerID, new ContainerRemoveParameters
            {
                Force = true,
                RemoveVolumes = true
            });
        }

        private async Task LoadImageAsync(GameInstance instance)
        {

            await this._dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = "factoriotools/factorio",
                Tag = FormatVersion(instance)
            }, new AuthConfig(), new ProgressSink());

        }

        private async Task<string> StartImageAsync(GameInstance instance, int port, string sourcePath)
        {
            CreateContainerResponse newContainer = null;
            try
            {
                newContainer = await this._dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
                {
                    Labels = new Dictionary<string, string>
                {
                    {DOCKER_LABEL_KEY, instance.Key }
                },
                    Volumes = new Dictionary<string, EmptyStruct> { { "/factorio", new EmptyStruct() } },
                    Image = "factoriotools/factorio:" + FormatVersion(instance),
                    ExposedPorts = new Dictionary<string, EmptyStruct> { { "34197/udp", new EmptyStruct() } },
                    Name = "factorio-server-" + instance.Key,
                    HostConfig = new HostConfig
                    {
                        PortBindings = new Dictionary<string, IList<PortBinding>> { { "34197/udp", new[] { new PortBinding { HostPort = string.Format("{0}", port) } } } },
                        Mounts = new List<Mount>
                    {
                        new Mount
                        {
                            Target = "/factorio",
                            Type="bind",
                            Source = sourcePath // TODO - get actual bind path
                        }
                    }
                    }
                });
            }
            catch (DockerApiException)
            {
                throw new Exception("Failed to start instance TODO");
            }


            await this._dockerClient.Containers.StartContainerAsync(newContainer.ID, new ContainerStartParameters());

            return newContainer.ID;
        }

        private string FormatVersion(GameInstance instance)
        {
            string version = string.Format("{0}.{1}", instance.TargetMajorVersion, instance.TargetMinorVersion);
            if (instance.TargetPatchVersion != null)
            {
                version = string.Format("{0}.{1}", version, instance.TargetPatchVersion);
            }

            return version;
        }

        private async Task<string> GetImageVersionAsync(string imageID)
        {
            if (this._imageVersionMap.TryGetValue(imageID, out string versionString))
            {
                return versionString;
            }

            ImageInspectResponse imageInfo = await this._dockerClient.Images.InspectImageAsync(imageID);

            return imageInfo.Config.Env.First(env => env.StartsWith("VERSION")).Split("=")[1];
        }
    }

    internal class ProgressSink : IProgress<JSONMessage>
    {
        public void Report(JSONMessage value)
        {
        }
    }
}
