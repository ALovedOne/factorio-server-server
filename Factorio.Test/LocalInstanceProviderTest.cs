using Factorio.Models;
using Factorio.Persistence;
using Factorio.Services.Interfaces;
using Factorio.Services.Persistence.LocalInstanceProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Factorio.Test
{
    public class LocalInstanceProviderTest : TestWithStuff
    {
        private readonly IInstanceProvider _instanceProvider;

        public LocalInstanceProviderTest() : base()
        {
            IOptions<LocalInstanceOptions> options = Options.Create(new LocalInstanceOptions { BaseDirectory = FullPath });
            ILogger<LocalInstanceProvider> logger = new Mock<ILogger<LocalInstanceProvider>>().Object;
            this._instanceProvider = new LocalInstanceProvider(options, logger);
        }

        [Fact]
        public void TestEnumeratesInstances()
        {
            this.AddTestSave("save_15_40");
            this.AddTestSave("save_17_50");
            this.AddBlankDir("blank_dir");

            List<GameInstance> all = new List<GameInstance>(this._instanceProvider.GetAll());

            Assert.Equal(3, all.Count);
        }

        [Fact]
        public void TestLoadById()
        {
            this.AddTestSave("save_15_40");
            this.AddTestSave("save_17_50");

            GameInstance testInstance = this._instanceProvider.GetById("save_17_50");
            Assert.NotNull(testInstance);
            Assert.Equal("save_17_50", testInstance.Key);
            Assert.Equal(17, testInstance.LastSave.Version.Minor);
        }

        [Fact]
        public void TestIdExists()
        {
            this.AddTestSave("save_17_50");

            Assert.True(this._instanceProvider.IdExists("save_17_50"));
            Assert.False(this._instanceProvider.IdExists("slug"));
        }

        [Fact]
        public void TestTryAddServerOverExisting()
        {
            this.AddBlankDir("existing-dir");

            Assert.False(this._instanceProvider.TryAddServer(new GameInstance
            {
                Name = "Existing Dir"
            }, out string newId));

            Assert.Equal("", newId);
        }

        [Fact]
        public void TestTryAddServerSuccessfully()
        {
            this.AddBlankDir("existing_dir");
            Assert.True(this._instanceProvider.TryAddServer(new GameInstance
            {
                Description = "New Description",
                Name = "New Dir",
                TargetVersion = new FuzzyVersion
                {
                    Major = 0,
                    Minor = 17,
                    Patch = 20
                }
            }, out string newId));
            Assert.Equal("new-dir", newId);

            GameInstance newServer = this._instanceProvider.GetById(newId);
            Assert.Equal("New Description", newServer.Description);
            Assert.Equal("New Dir", newServer.Name);
            Assert.Equal(0, newServer.TargetVersion.Major);
            Assert.Equal(17, newServer.TargetVersion.Minor);
            Assert.Equal(20, newServer.TargetVersion.Patch);
            Assert.Equal(Path.Combine(this.FullPath, "new-dir"),
                newServer.ImplementationInfo.GetValueOrDefault("localPath"));
        }

        [Fact]
        public void TestUpdateServer()
        {
            const string newServerName = "New Server Name";
            const string newServerDescription = "New Server Description";
            GameInstance game = this.AddTestSave("save_17_50");

            this._instanceProvider.UpdateServer("save_17_50", new GameInstance { Name = newServerName, Description = newServerDescription, TargetVersion = game.TargetVersion });
            GameInstance testInstance = this._instanceProvider.GetById("save_17_50");

            Assert.Equal(newServerName, testInstance.Name);
            Assert.Equal(newServerDescription, testInstance.Description);
        }

        [Fact]
        public void TestLoadingSaveDir()
        {
            this.AddTestSave("save_17_50");

            List<GameInstance> all = new List<GameInstance>(this._instanceProvider.GetAll());
            Assert.Single(all);

            GameInstance testInstance = all[0];
            Assert.Equal(0, testInstance.LastSave.Version.Major);
            Assert.Equal(17, testInstance.LastSave.Version.Minor);
            Assert.Equal(50, testInstance.LastSave.Version.Patch);
        }

        [Fact]
        public void TestLoadingBlankDir()
        {
            this.AddBlankDir("blank_dir");

            List<GameInstance> all = new List<GameInstance>(this._instanceProvider.GetAll());
            Assert.Single(all);
        }
    }
}
