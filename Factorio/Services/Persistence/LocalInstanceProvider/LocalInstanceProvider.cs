using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;

//https://hub.docker.com/v2/repositories/factoriotools/factorio/tags/
// TODO - concurrency
namespace Factorio.Services.Persistence.LocalInstanceProvider
{
    public class LocalInstanceProvider : AbstractFileSystemInstanceProvider
    {
        #region ctor
        public LocalInstanceProvider(IOptionsMonitor<LocalInstanceOptions> optionsAccessor) :
            this(optionsAccessor.CurrentValue.BaseDirectory)
        {
        }

        public LocalInstanceProvider(string serverBaseDirectoryPath)
            : base(new DirectoryInfo(serverBaseDirectoryPath))

        {            
        }
        #endregion

        
        public override IReadOnlyDictionary<string, string> GetImplementationInfo(string key)
        {
            return new Dictionary<string, string>
            {
                { "localPath", Path.Combine(this._baseDirectory.FullName, key) }
            };
        }
    }
}
