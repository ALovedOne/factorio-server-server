using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace Factorio.Services.Persistence.FileSystems
{
    public class NFSInstanceProvider : AbstractFileSystemInstanceProvider
    {
        public NFSInstanceProvider(IOptions<InstanceProviderOptions> options, ILogger<NFSInstanceProvider> logger)
           : base(new DirectoryInfo(options.Value.BaseDirectory),
                 new Uri("nfs://" + options.Value.HostName + "/" + options.Value.MountDirectory + "/"),
                 logger)
        { }
    }
}
