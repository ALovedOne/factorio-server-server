using Factorio.Models;
using Factorio.Services.Interfaces;
using Factorio.Services.Persistence;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Factorio.Test
{
    public class DockerBindInstanceProviderTest : TestWithStuff
    {
        private readonly IInstanceProvider _instanceProvider;
        private static string RemoteDirectory = "C:\\Factorio";

        public DockerBindInstanceProviderTest():base()
        {
            this._instanceProvider = new DockerBindInstanceProvider(FullPath, RemoteDirectory);
        }

        [Fact]
        public void LocalDirectoryIsRelativeToHost()
        {
            this.AddTestSave("save_17_50");
            GameInstance instance = this._instanceProvider.GetById("save_17_50");
            Assert.Equal(Path.Combine(RemoteDirectory, "save_17_50"), instance.ImplementationInfo.GetValueOrDefault("localPath"));
        }
    }
}
