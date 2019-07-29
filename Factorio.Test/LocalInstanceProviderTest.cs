using System;
using System.IO;
using System.IO.Compression;
using Xunit;


using factorio.Persistence;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using factorio.Models;

namespace Factorio.Test
{
    public class LocalInstanceProviderTest : IDisposable
    {
        private DirectoryInfo _testDir;
        private IInstanceProvider _instanceProvider;

        public LocalInstanceProviderTest()
        {
            DirectoryInfo userTempDir = new DirectoryInfo(Path.GetTempPath());
            string randomName = Path.GetRandomFileName();

            this._testDir = userTempDir.CreateSubdirectory(randomName);

            IConfigurationRoot configRoot = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("LocalPersistenceProvider:BaseDirectory",this._testDir.FullName)
                })
                .Build();
            this._instanceProvider = new LocalInstanceProvider(configRoot);
        }

        public void Dispose()
        {
            // If _testDir exists, delete it
            if (this._testDir.Exists)
            {
                this._testDir.Delete(true);
            }
        }


        [Fact]
        public void TestEnumeratesInstances()
        {
            this.addTestSave("save_15_40");
            this.addTestSave("save_17_50");
            this.addBlankDir("blank_dir");

            List<Server> all = new List<Server>(this._instanceProvider.getAll());

            Assert.Equal(3, all.Count);
        }

        [Fact]
        public void TestLoadById()
        {
            this.addTestSave("save_15_40");
            this.addTestSave("save_17_50");

            Server testInstance = this._instanceProvider.getById("save_17_50");
            Assert.NotNull(testInstance);
            Assert.Equal("save_17_50", testInstance.Slug);
            Assert.Equal(17, testInstance.LastSaveMajorVersion);
        }

        [Fact]
        public void TestIdExists()
        {
            this.addTestSave("save_17_50");

            Assert.True(this._instanceProvider.idExists("save_17_50"));
            Assert.False(this._instanceProvider.idExists("slug"));
        }

        // tryAddServer
        [Fact]
        public void TestTryAddServerOverExisting()
        {
            string newId;

            this.addBlankDir("existing-dir");

            Assert.False(this._instanceProvider.tryAddServer(new Server
            {
                Name = "Existing Dir"
            }, out newId));

            Assert.Equal("", newId);
        }

        [Fact]
        public void TestTryAddServerSuccessfully()
        {
            string newId;

            this.addBlankDir("existing_dir");
            Assert.True(this._instanceProvider.tryAddServer(new Server
            {
                Description = "New Description",
                Name = "New Dir",
                TargetMajorVersion = 17,
                TargetMinorVersion = 20
            }, out newId));
            Assert.Equal("new-dir", newId);

            Server newServer = this._instanceProvider.getById(newId);
            Assert.Equal("New Description", newServer.Description);
            Assert.Equal("New Dir", newServer.Name);
            Assert.Equal(17, newServer.TargetMajorVersion);
            Assert.Equal(20, newServer.TargetMinorVersion);
        }

        // updateServer
        [Fact]
        public void TestUpdateServer()
        {
            const string newServerName = "New Server Name";
            const string newServerDescription = "New Server Description";
            this.addTestSave("save_17_50");

            this._instanceProvider.updateServer("save_17_50", new Server { Name = newServerName, Description = newServerDescription });
            Server testInstance = this._instanceProvider.getById("save_17_50");

            Assert.Equal(newServerName, testInstance.Name);
            Assert.Equal(newServerDescription, testInstance.Description);
        }

        [Fact]
        public void TestLoadingSaveDir()
        {
            this.addTestSave("save_17_50");

            List<Server> all = new List<Server>(this._instanceProvider.getAll());
            Assert.Single(all);

            Server testInstance = all[0];
            Assert.Equal(17, testInstance.LastSaveMajorVersion);
            Assert.Equal(50, testInstance.LastSaveMinorVersion);
        }

        [Fact]
        public void TestLoadingBlankDir()
        {
            this.addBlankDir("blank_dir");

            List<Server> all = new List<Server>(this._instanceProvider.getAll());
            Assert.Single(all);
        }




        private void addTestSave(string saveFile)
        {
            string pathAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string folderAssembly = Path.GetDirectoryName(pathAssembly);

            string zipFileName = Path.Combine(folderAssembly, "TestData", saveFile + ".zip");
            ZipFile.ExtractToDirectory(zipFileName, this._testDir.FullName);
        }

        private void addBlankDir(string dirName)
        {
            this._testDir.CreateSubdirectory(dirName);
        }


    }
}