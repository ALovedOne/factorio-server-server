using Docker.DotNet;
using Docker.DotNet.Models;
using Factorio.Models;
using Factorio.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Factorio.Services.Execution.DockerImplementation
{
    public class DockerRunnerService : IRunnerService
    {
        private const string DOCKER_LABEL_KEY = "factorio-server-key";

        private readonly ConcurrentDictionary<string, string> _imageVersionMap = new ConcurrentDictionary<string, string>();

        private readonly DockerClient _dockerClient;
        private readonly int _portRangeStart;
        private readonly int _portRangeEnd;


        /*
         * Adding pre-emtive port checking
         *   - Add dictionary, lock dict, check port, if valid add, unlock, try to start, if fails, remove, when stopped remove
         */
        public DockerRunnerService(IOptions<DockerExecutionOptions> options)
        {
            string defaultUrl =
                Environment.OSVersion.Platform == PlatformID.Unix ?
                    "unix:/var/run/docker.sock" :
                    "npipe://./pipe/docker_engine";
            string dockerURL = options != null ? options.Value.DockerUrl : null;

            this._dockerClient = new DockerClientConfiguration(new Uri(dockerURL ?? defaultUrl)).CreateClient();
            this._portRangeStart = options.Value.PortRangeBegin;
            this._portRangeEnd = options.Value.PortRangeEnd;
        }

        #region Getters
        /// <summary>
        /// Gets all instances of the game (started by this program) running on docker
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ExecutionInfo>> GetExecutionInfosAsync()
        {
            return await GetExecutionInfosFromDockerAsync();
        }

        /// <summary>
        /// Gets the runing instance based on the key of the instance being run
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ExecutionInfo> GetByExecutionInfoKeyAsync(string key)
        {
            IList<ExecutionInfo> instances = await GetExecutionInfosFromDockerAsync(key);

            if (instances == null) return null;
            if (instances.Count != 1) return null; // TODO - return something else maybe

            return instances[0];
        }
        #endregion

        #region Container control
        public async Task<ExecutionInfo> StartInstanceAsync(string host, int port, GameInstance instance)
        {
            string version;

            if (port < this._portRangeStart || port > this._portRangeEnd)
            {
                // TODO - also check other instances (not trivial due to race condition)
                throw new Exception("TODO - Invalid port");
            }

            if (instance.TargetVersion.Patch == null)
                version = string.Format("{0}.{1}", instance.TargetVersion.Major, instance.TargetVersion.Minor);
            else
                version = string.Format("{0}.{1}.{2}", instance.TargetVersion.Major, instance.TargetVersion.Minor, instance.TargetVersion.Patch);

            await this.LoadImageAsync(version);
            // TODO - handle invliad info
            string newID = await this.StartImageAsync(instance.Key, port, instance.ImplementationInfo.GetValueOrDefault("localPath", ""), version);

            ExecutionInfo i = new ExecutionInfo
            {
                Key = newID,
                Hostname = host,
                Port = port,

                RunningVersion = version,
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
        #endregion

        private async Task<string> GetContainerIDAsync(int? port = null, string key = null)
        {
            IList<ExecutionInfo> instances = await GetExecutionInfosFromDockerAsync(key, port);

            if (instances == null) return null;
            if (instances.Count > 1) return null; // TODO - handle multiple
            return instances[0].Key;
        }


        private async Task StopContainerByIDAsync(string containerID)
        {
            await this._dockerClient.Containers.RemoveContainerAsync(containerID, new ContainerRemoveParameters
            {
                Force = true,
                RemoveVolumes = true
            });
        }

        private async Task LoadImageAsync(string version)
        {

            await this._dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = "factoriotools/factorio",
                Tag = version
            }, new AuthConfig(), new ProgressSink());

        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="port"></param>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        private async Task<string> StartImageAsync(string key, int port, string sourcePath, string version)
        {
            CreateContainerResponse newContainer = null;
            try
            {
                newContainer = await this._dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
                {
                    Labels = new Dictionary<string, string>
                {
                    {DOCKER_LABEL_KEY, key }
                },
                    Volumes = new Dictionary<string, EmptyStruct> { { "/factorio", new EmptyStruct() } },
                    Image = "factoriotools/factorio:" + version,
                    ExposedPorts = new Dictionary<string, EmptyStruct> { { "34197/udp", new EmptyStruct() } },
                    Name = "factorio-server-" + key,
                    HostConfig = new HostConfig
                    {
                        PortBindings = new Dictionary<string, IList<PortBinding>> { { "34197/udp", new[] { new PortBinding { HostPort = string.Format("{0}", port) } } } },
                        Mounts = new List<Mount>
                    {
                        new Mount
                        {
                            Target = "/factorio",
                            Type="bind",
                            Source = sourcePath
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

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private async Task<IList<ExecutionInfo>> GetExecutionInfosFromDockerAsync(string instanceKey = null, int? port = null)
        {
            IDictionary<string, IDictionary<string, bool>> filters = new Dictionary<string, IDictionary<string, bool>>
            { { "label", new Dictionary<string, bool> { { DOCKER_LABEL_KEY + (instanceKey == null ? "" : "=" + instanceKey), true } } } };

            if (port != null) filters.Add("publish", new Dictionary<string, bool> { { string.Format("{0}", port), true } });

            IEnumerable<ContainerListResponse> ExecutionInfos = await this._dockerClient.Containers.ListContainersAsync(new ContainersListParameters { Filters = filters, All = true });
            await LoadImageInfo(ExecutionInfos);

            return new List<ExecutionInfo>(ExecutionInfos.Select(containerInfo => MapDockerContainer(containerInfo)));
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="responses"></param>
        /// <returns></returns>
        private async Task LoadImageInfo(IEnumerable<ContainerListResponse> responses)
        {
            Task[] tasks = responses.Select(async r =>
            {
                string version = await GetImageVersionAsync(r.ImageID);
            }).ToArray();

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="containerInfo"></param>
        /// <returns></returns>
        private ExecutionInfo MapDockerContainer(ContainerListResponse containerInfo)
        {
            Port exposedPort = containerInfo.Ports.FirstOrDefault(p => p.PrivatePort == 34197);
            string runningVersion;
            TryGetImageVersion(containerInfo.ImageID, out runningVersion);

            return new ExecutionInfo
            {
                Key = containerInfo.ID,
                Hostname = "localhost",
                Port = exposedPort != null ? exposedPort.PublicPort : 0,
                RunningVersion = runningVersion,
                InstanceKey = containerInfo.Labels[DOCKER_LABEL_KEY]
            };
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="imageID"></param>
        /// <returns></returns>
        private async Task<string> GetImageVersionAsync(string imageID)
        {
            if (this._imageVersionMap.TryGetValue(imageID, out string versionString))
                return versionString;

            ImageInspectResponse imageInfo = await this._dockerClient.Images.InspectImageAsync(imageID);
            string version = imageInfo.Config.Env.First(env => env.StartsWith("VERSION")).Split("=")[1];

            if (!this._imageVersionMap.ContainsKey(imageID))
                this._imageVersionMap.TryAdd(imageID, version);
            return version;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="imageID"></param>
        /// <param name="versionString"></param>
        /// <returns></returns>
        private bool TryGetImageVersion(string imageID, out string versionString)
        {
            return this._imageVersionMap.TryGetValue(imageID, out versionString);
        }
    }

    internal class ProgressSink : IProgress<JSONMessage>
    {
        public void Report(JSONMessage value)
        {
        }
    }
}
