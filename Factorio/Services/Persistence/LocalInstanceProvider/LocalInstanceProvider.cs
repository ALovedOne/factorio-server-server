using Microsoft.Extensions.Logging;
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
        public LocalInstanceProvider(IOptions<LocalInstanceOptions> optionsAccessor, ILogger<LocalInstanceProvider> logger) :
            base(new DirectoryInfo(optionsAccessor.Value.BaseDirectory), logger)
        {
        }
        #endregion

        public override string ConfigBaseDir(string key)
        {
            return Path.Combine(this._baseDirectory.FullName, key);
        }
    }
}
