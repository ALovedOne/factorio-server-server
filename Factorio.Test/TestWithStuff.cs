using Factorio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Factorio.Test
{
    public class TestWithStuff : IDisposable
    {
        private readonly DirectoryInfo _testDir;

        public TestWithStuff()
        {
            DirectoryInfo userTempDir = new DirectoryInfo(Path.GetTempPath());
            string randomName = Path.GetRandomFileName();

            this._testDir = userTempDir.CreateSubdirectory(randomName);
        }

        public void Dispose()
        {
            if (this._testDir.Exists)
            {
                this._testDir.Delete(true);
            }
        }

        protected string FullPath { get { return this._testDir.FullName; } }

        protected GameInstance AddTestSave(string saveFile, int? patchVersion = 50)
        {
            string pathAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string folderAssembly = Path.GetDirectoryName(pathAssembly);

            string zipFileName = Path.Combine(folderAssembly, "TestData", saveFile + ".zip");
            ZipFile.ExtractToDirectory(zipFileName, this._testDir.FullName);

            return new GameInstance
            {
                Key = "test-server-" + saveFile,
                ImplementationInfo = new Dictionary<string, string> { { "localPath", Path.Combine(this._testDir.FullName, saveFile) } },
                TargetVersion = new FuzzyVersion
                {
                    Major = 0,
                    Minor = 17,
                    Patch = patchVersion
                }
            };
        }

        protected GameInstance AddBlankDir(string dirName, int? PatchVersion = null)
        {
            DirectoryInfo d = this._testDir.CreateSubdirectory(dirName);
            return new GameInstance
            {
                Key = "test-server-" + dirName,
                ImplementationInfo = new Dictionary<string, string> { { "localPath", d.FullName } },
                TargetVersion = new FuzzyVersion
                {
                    Major = 0,
                    Minor = 17,
                    Patch = PatchVersion
                }
            };
        }
    }
}
