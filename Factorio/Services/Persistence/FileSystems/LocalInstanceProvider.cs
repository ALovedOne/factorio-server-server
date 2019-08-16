using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;

//https://hub.docker.com/v2/repositories/factoriotools/factorio/tags/
// TODO - concurrency

namespace Factorio.Services.Persistence.FileSystems
{
    public class LocalInstanceProvider : AbstractFileSystemInstanceProvider
    {
        #region ctor
        public LocalInstanceProvider(IOptions<InstanceProviderOptions> options, ILogger<LocalInstanceProvider> logger) :
            base(new DirectoryInfo(options.Value.BaseDirectory), new Uri("file:///" + options.Value.BaseDirectory + "/"), logger)
        {
        }
        #endregion
    }
}
