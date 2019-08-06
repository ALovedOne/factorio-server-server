using Factorio.Services.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;

namespace Factorio.Services.Persistence
{
    public class DockerBindInstanceProvider : AbstractFileSystemInstanceProvider
    {
        private readonly string _hostPath;

        public DockerBindInstanceProvider(IOptions<DockerBindInstanceOptions> options, ILogger<DockerBindInstanceProvider> logger)
            : base(new DirectoryInfo(options.Value.BoundBaseDirectory), logger)
        {
            _hostPath = options.Value.HostBaseDirectory;
        }
        
        public override IReadOnlyDictionary<string, string> GetImplementationInfo(string key)
        {
            return new Dictionary<string, string> { { "localPath", Path.Combine(_hostPath, key) } };
        }
    }
}
