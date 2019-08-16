using Docker.DotNet;
using Docker.DotNet.Models;
using Factorio.Models;
using Factorio.Services.Interfaces;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;
        private readonly string _publicHost;

        private readonly string[] _knownSchemes = { "file", "nfs" };

        /*
         * Adding pre-emtive port checking
         *   - Add dictionary, lock dict, check port, if valid add, unlock, try to start, if fails, remove, when stopped remove
         */
        public DockerRunnerService(IOptions<DockerExecutionOptions> options, ILogger<DockerRunnerService> logger)
        {
            string defaultUrl =
                Environment.OSVersion.Platform == PlatformID.Unix ?
                    "unix:/var/run/docker.sock" :
                    "npipe://./pipe/docker_engine";
            string dockerURL = options?.Value.DockerUrl;

            _dockerClient = new DockerClientConfiguration(new Uri(dockerURL ?? defaultUrl)).CreateClient();
            _portRangeStart = options.Value.PortRangeBegin;
            _portRangeEnd = options.Value.PortRangeEnd;
            _publicHost = options.Value.DockerPublicHostName;

            _logger = logger;

            logger.LogInformation("Docker URL: {url} Port Range: {portStart} - {portEnd}", dockerURL ?? defaultUrl, this._portRangeStart, this._portRangeEnd);
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
            if (instances.Count != 1)
            {
                _logger.LogWarning("Multiple keys for {ImageKey}", key);
                return null;
            }

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
                this._logger.LogWarning("Port ({port}) out of range ({low} - {high})", port, this._portRangeStart, this._portRangeEnd);
                throw new Exception("TODO - Invalid port");
            }

            if (!_knownSchemes.Contains(instance.ConfigUrl.Scheme))
            {
                this._logger.LogWarning("Config URL Scheme ({scheme}) is not valid", instance.ConfigUrl.Scheme);
            }

            if (instance.LastSaveUrl != null && !_knownSchemes.Contains(instance.LastSaveUrl.Scheme))
            {
                this._logger.LogWarning("Last Save URL Scheme ({scheme}) is not valid", instance.LastSaveUrl.Scheme);
            }

            if (instance.TargetVersion.Patch == null)
                version = string.Format("{0}.{1}", instance.TargetVersion.Major, instance.TargetVersion.Minor);
            else
                version = string.Format("{0}.{1}.{2}", instance.TargetVersion.Major, instance.TargetVersion.Minor, instance.TargetVersion.Patch);

            await this.LoadImageAsync(version);
            // TODO - handle invliad info
            string newID = await this.StartImageAsync(instance.Key, port, instance.ConfigUrl, version);

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
        private async Task<string> StartImageAsync(string key, int port, Uri configUri, string version)
        {
            this._logger.LogInformation("Trying to start image {imageKey} {version} on {port} with {URL}", key, version, port, configUri);

            VolumesCreateParameters r = CreateVolumeRequest(key, configUri);

            if (r != null)
            {
                await _dockerClient.Volumes.RemoveAsync(r.Name, true);
                VolumeResponse vol = await this._dockerClient.Volumes.CreateAsync(CreateVolumeRequest(key, configUri));
            }

            Mount factorioMount = ConfigUrlToMount(key, configUri);

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
                        Mounts = new List<Mount> { factorioMount }
                    }
                });
            }
            catch (DockerApiException e)
            {
                throw new Exception("Failed to start instance", e);
            }

            await this._dockerClient.Containers.StartContainerAsync(newContainer.ID, new ContainerStartParameters());

            return newContainer.ID;
        }

        /// <summary>
        /// Creates and returns a Docker request to create a new volume if needed
        /// </summary>
        /// <param name="gameKey"></param>
        /// <param name="configDir"></param>
        /// <returns></returns>
        public static VolumesCreateParameters CreateVolumeRequest(string gameKey, Uri configDir)
        {
            if (configDir.Scheme == "nfs")
            {
                return new VolumesCreateParameters
                {
                    Driver = "local",
                    Name = string.Format("factorio-volume-{0}", gameKey),
                    DriverOpts = new Dictionary<string, string> { { "device", configDir.AbsolutePath }, { "o", string.Format("addr={0},vers=4,soft", configDir.Host) }, { "type", "nfs" } },
                    Labels = new Dictionary<string, string> { { "factorio-game-key", gameKey } },
                };
            }
            return null;
        }

        /// <summary>
        /// Creates the Mount directive for the docker create
        /// </summary>
        /// <param name="configUri"></param>
        /// <param name="vol"></param>
        /// <returns></returns>
        public static Mount ConfigUrlToMount(string gameKey, Uri configUri)
        {
            if (configUri.Scheme == "file")
            {
                return new Mount
                {
                    Target = "/factorio",
                    Type = "bind",
                    Source = configUri.AbsolutePath
                };
            }
            else if (configUri.Scheme == "nfs")
            {
                return new Mount
                {
                    Type = "volume",
                    Target = "/factorio",
                    Source = string.Format("factorio-volume-{0}", gameKey)
                };
            }
            else
            {
                return null;

            }
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
            TryGetImageVersion(containerInfo.ImageID, out string runningVersion);

            return new ExecutionInfo
            {
                Key = containerInfo.ID,
                Hostname = this._publicHost,
                Port = exposedPort != null ? exposedPort.PublicPort : 0,
                RunningVersion = runningVersion,
                InstanceKey = containerInfo.Labels[DOCKER_LABEL_KEY],
                Status = containerInfo.State == "exited" ? ExecutionStatus.Stoppped : ExecutionStatus.Started
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
