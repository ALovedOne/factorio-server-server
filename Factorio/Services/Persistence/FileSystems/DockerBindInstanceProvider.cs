using Factorio.Services.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace Factorio.Services.Persistence.FileSystems
{
    public class DockerBindInstanceProvider : AbstractFileSystemInstanceProvider
    {
        public DockerBindInstanceProvider(IOptions<InstanceProviderOptions> options, ILogger<DockerBindInstanceProvider> logger)
            : base(new DirectoryInfo(options.Value.BaseDirectory), new Uri("file:///" + options.Value.MountDirectory), logger)
        {
        }
    }
}
