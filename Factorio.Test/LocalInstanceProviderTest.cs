using Factorio.Persistence;
using Factorio.Persistence.Interfaces;
using Factorio.Persistence.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Xunit;

namespace Factorio.Test
{
    public class LocalInstanceProviderTest : TestWithStuff
    {
        private readonly IInstanceProvider _instanceProvider;

        public LocalInstanceProviderTest() : base()
        {
            IConfigurationRoot configRoot = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("LocalPersistenceProvider:BaseDirectory",this.FullPath)
                })
                .Build();
            this._instanceProvider = new LocalInstanceProvider(configRoot);
        }

        [Fact]
        public void TestEnumeratesInstances()
        {
            this.AddTestSave("save_15_40");
            this.AddTestSave("save_17_50");
            this.AddBlankDir("blank_dir");

            List<IInstance> all = new List<IInstance>(this._instanceProvider.GetAll());

            Assert.Equal(3, all.Count);
        }

        [Fact]
        public void TestLoadById()
        {
            this.AddTestSave("save_15_40");
            this.AddTestSave("save_17_50");

            Instance testInstance = this._instanceProvider.GetById("save_17_50") as Instance;
            Assert.NotNull(testInstance);
            Assert.Equal("save_17_50", testInstance.Key);
            Assert.Equal(17, testInstance.LastSave.MinorVersion);
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

            Assert.False(this._instanceProvider.TryAddServer(new Instance
            {
                Name = "Existing Dir"
            }, out string newId));

            Assert.Equal("", newId);
        }

        [Fact]
        public void TestTryAddServerSuccessfully()
        {
            this.AddBlankDir("existing_dir");
            Assert.True(this._instanceProvider.TryAddServer(new Instance
            {
                Description = "New Description",
                Name = "New Dir",
                TargetMajorVersion = 17,
                TargetMinorVersion = 20
            }, out string newId));
            Assert.Equal("new-dir", newId);

            Instance newServer = this._instanceProvider.GetById(newId) as Instance;
            Assert.Equal("New Description", newServer.Description);
            Assert.Equal("New Dir", newServer.Name);
            Assert.Equal(17, newServer.TargetMinorVersion);
            Assert.Equal(20, newServer.TargetPatchVersion);
        }

        [Fact]
        public void TestUpdateServer()
        {
            const string newServerName = "New Server Name";
            const string newServerDescription = "New Server Description";
            this.AddTestSave("save_17_50");

            this._instanceProvider.UpdateServer("save_17_50", new Instance { Name = newServerName, Description = newServerDescription });
            Instance testInstance = this._instanceProvider.GetById("save_17_50") as Instance;

            Assert.Equal(newServerName, testInstance.Name);
            Assert.Equal(newServerDescription, testInstance.Description);
        }

        [Fact]
        public void TestLoadingSaveDir()
        {
            this.AddTestSave("save_17_50");

            List<IInstance> all = new List<IInstance>(this._instanceProvider.GetAll());
            Assert.Single(all);

            IInstance testInstance = all[0];
            Assert.Equal(0, testInstance.LastSave.MajorVersion);
            Assert.Equal(17, testInstance.LastSave.MinorVersion);
            Assert.Equal(50, testInstance.LastSave.PatchVersion);
        }

        [Fact]
        public void TestLoadingBlankDir()
        {
            this.AddBlankDir("blank_dir");

            List<IInstance> all = new List<IInstance>(this._instanceProvider.GetAll());
            Assert.Single(all);
        }
    }
}
