using Factorio.Models;
using Factorio.Services.Interfaces;
using Factorio.Services.Persistence;
using Factorio.Services.Persistence.FileSystems;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Factorio.Test
{
    public class LocalInstanceProviderTest : TestWithTempDir
    {
        private readonly IInstanceProvider _instanceProvider;

        public LocalInstanceProviderTest() : base()
        {
            IOptions<InstanceProviderOptions> options = Options.Create(new InstanceProviderOptions { BaseDirectory = FullPath });
            ILogger<LocalInstanceProvider> logger = new Mock<ILogger<LocalInstanceProvider>>().Object;
            _instanceProvider = new LocalInstanceProvider(options, logger);
        }

        [Fact]
        public void TestEnumeratesInstances()
        {
            AddTestSave("save_15_40");
            AddTestSave("save_17_50");
            AddBlankDir("blank_dir");

            List<GameInstance> all = new List<GameInstance>(_instanceProvider.GetAll());

            Assert.Equal(3, all.Count);
        }

        [Fact]
        public void TestLoadById()
        {
            AddTestSave("save_15_40");
            AddTestSave("save_17_50");

            GameInstance testInstance = _instanceProvider.GetById("save_17_50");
            Assert.NotNull(testInstance);
            Assert.Equal("save_17_50", testInstance.Key);
            Assert.Equal(17, testInstance.LastSave.Version.Minor);
        }

        [Fact]
        public void TestIdExists()
        {
            AddTestSave("save_17_50");

            Assert.True(_instanceProvider.IdExists("save_17_50"));
            Assert.False(_instanceProvider.IdExists("slug"));
        }

        [Fact]
        public void TestTryAddServerOverExisting()
        {
            AddBlankDir("existing-dir");

            GameInstance newInstance = _instanceProvider.TryAddServer(new GameInstance
            {
                Name = "Existing Dir"
            });

            Assert.Null(newInstance);
        }

        [Fact]
        public void TestTryAddServerSuccessfully()
        {
            AddBlankDir("existing_dir");
            GameInstance newInstance = _instanceProvider.TryAddServer(new GameInstance
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

            Assert.Equal(Path.Combine(FullPath, "new-dir/").Replace("\\", "/"), newInstance.ConfigUrl.AbsolutePath);
            Assert.Null(newInstance.LastSaveUrl);
        }

        [Fact]
        public void TestUpdateServer()
        {
            const string newServerName = "New Server Name";
            const string newServerDescription = "New Server Description";
            AddTestSave("save_17_50");
            GameInstance desiredState = new GameInstance { Key = "save_17_50", Name = newServerName, Description = newServerDescription, TargetVersion = new FuzzyVersion { Major = 0, Minor = 17, Patch = 50 } };

            GameInstance updatedGame = _instanceProvider.UpdateServer(desiredState);
            Assert.NotNull(updatedGame);

            Assert.Equal(newServerName, updatedGame.Name);
            Assert.Equal(newServerDescription, updatedGame.Description);
        }

        [Fact]
        public void TestLoadingSaveDir()
        {
            AddTestSave("save_17_50");

            List<GameInstance> all = new List<GameInstance>(_instanceProvider.GetAll());
            Assert.Single(all);

            GameInstance testInstance = all[0];
            Assert.Equal(0, testInstance.LastSave.Version.Major);
            Assert.Equal(17, testInstance.LastSave.Version.Minor);
            Assert.Equal(50, testInstance.LastSave.Version.Patch);
        }

        [Fact]
        public void TestLoadingBlankDir()
        {
            AddBlankDir("blank_dir");

            List<GameInstance> all = new List<GameInstance>(_instanceProvider.GetAll());
            Assert.Single(all);
        }

        [Fact]
        public void TestLoadingRecentSave()
        {
            AddTestSave("save_15_40");

            GameInstance g = _instanceProvider.GetById("save_15_40");
            Assert.EndsWith("_autosave2.zip", g.LastSaveUrl.AbsolutePath);
        }

    }
}
