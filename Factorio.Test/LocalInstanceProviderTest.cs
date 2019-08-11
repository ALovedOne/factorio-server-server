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

            GameInstance newInstance = this._instanceProvider.TryAddServer(new GameInstance
            {
                Name = "Existing Dir"
            });

            Assert.Null(newInstance);
        }

        [Fact]
        public void TestTryAddServerSuccessfully()
        {
            this.AddBlankDir("existing_dir");
            GameInstance newInstance = this._instanceProvider.TryAddServer(new GameInstance
            {
                Description = "New Description",
                Name = "New Dir",
                TargetVersion = new FuzzyVersion
                {
                    Major = 0,
                    Minor = 17,
                    Patch = 20
                }
            });
            Assert.NotNull(newInstance);
            Assert.Equal("new-dir", newInstance.Key);

            Assert.Equal("New Description", newInstance.Description);
            Assert.Equal("New Dir", newInstance.Name);
            Assert.Equal(0, newInstance.TargetVersion.Major);
            Assert.Equal(17, newInstance.TargetVersion.Minor);
            Assert.Equal(20, newInstance.TargetVersion.Patch);
            Assert.Equal(Path.Combine(this.FullPath, "new-dir"),
                newInstance.ImplementationInfo.GetValueOrDefault("localPath"));
        }

        [Fact]
        public void TestUpdateServer()
        {
            const string newServerName = "New Server Name";
            const string newServerDescription = "New Server Description";
            GameInstance game = this.AddTestSave("save_17_50");
            GameInstance desciredState = new GameInstance { Key = "save_17_50", Name = newServerName, Description = newServerDescription, TargetVersion = game.TargetVersion };

            GameInstance updatedGame = this._instanceProvider.UpdateServer(desciredState);
            Assert.NotNull(updatedGame);

            Assert.Equal(newServerName, updatedGame.Name);
            Assert.Equal(newServerDescription, updatedGame.Description);
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
