using Factorio.Services.Persistence;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;

namespace Factorio.Services.Persistence
{
    public class DockerBindInstanceProvider : AbstractFileSystemInstanceProvider
    {
        private readonly string _hostPath;

        public DockerBindInstanceProvider(IOptions<DockerBindInstanceOptions> options)
            : this(options.Value.BoundBaseDirectory, options.Value.HostBaseDirectory)   
        {
        }

        public DockerBindInstanceProvider(string boundBaseDirectory, string hostBaseDirectory)
            :            base(new DirectoryInfo(boundBaseDirectory))
        {
            _hostPath = hostBaseDirectory;
        }

        public override IReadOnlyDictionary<string, string> GetImplementationInfo(string key)
        {
            return new Dictionary<string, string> { { "localPath", Path.Combine(_hostPath, key) } };
        }
    }
}
