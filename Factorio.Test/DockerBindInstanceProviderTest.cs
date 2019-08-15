using Factorio.Models;
using Factorio.Services.Interfaces;
using Factorio.Services.Persistence;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;

namespace Factorio.Test
{
    public class DockerBindInstanceProviderTest : TestWithTempDir
    {
        private readonly IInstanceProvider _instanceProvider;
        private static string RemoteDirectory = "C:\\Factorio";

        public DockerBindInstanceProviderTest() : base()
        {
            IOptions<DockerBindInstanceOptions> options = Options.Create(new DockerBindInstanceOptions { BoundBaseDirectory = FullPath, HostBaseDirectory = RemoteDirectory });
            ILogger<DockerBindInstanceProvider> logger = new Mock<ILogger<DockerBindInstanceProvider>>().Object;
            _instanceProvider = new DockerBindInstanceProvider(options, logger);
        }

        [Fact]
        public void LocalDirectoryIsRelativeToHost()
        {
            AddTestSave("save_17_50");
            GameInstance instance = _instanceProvider.GetById("save_17_50");

            Assert.Equal("file", instance.ConfigUrl.Scheme);
            Assert.Equal(Path.Combine(RemoteDirectory, "save_17_50"), instance.ConfigUrl.AbsolutePath);

            Assert.Equal("file", instance.LastSaveUrl.Scheme);
            Assert.Equal(Path.Combine(RemoteDirectory, "save_17_50"), instance.LastSaveUrl.AbsolutePath);
        }
    }
}
