using Factorio.Models;
using Factorio.Services.Interfaces;
using Factorio.Services.Persistence.FileSystems;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.IO;
using Xunit;

namespace Factorio.Test
{
    public class DockerBindInstanceProviderTest : TestWithTempDir
    {
        private readonly IInstanceProvider _instanceProvider;
        private static string RemoteDirectory = "C:/Factorio/";

        public DockerBindInstanceProviderTest() : base()
        {
            IOptions<InstanceProviderOptions> options = Options.Create(new InstanceProviderOptions { BaseDirectory = FullPath, MountDirectory = RemoteDirectory });
            ILogger<DockerBindInstanceProvider> logger = new Mock<ILogger<DockerBindInstanceProvider>>().Object;
            _instanceProvider = new DockerBindInstanceProvider(options, logger);
        }

        [Fact]
        public void LocalDirectoryIsRelativeToHost()
        {
            AddTestSave("save_17_50");
            GameInstance instance = _instanceProvider.GetById("save_17_50");

            Assert.Equal("file", instance.ConfigUrl.Scheme);
            Assert.Equal("C:/Factorio/save_17_50/", instance.ConfigUrl.AbsolutePath);

            Assert.Equal("file", instance.LastSaveUrl.Scheme);
            Assert.Equal("C:/Factorio/save_17_50/saves/_autosave1.zip", instance.LastSaveUrl.AbsolutePath);
        }
    }
}
