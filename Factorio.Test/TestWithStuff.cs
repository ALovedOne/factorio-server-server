using Factorio.Persistence.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

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

        protected Instance AddTestSave(string saveFile)
        {
            string pathAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string folderAssembly = Path.GetDirectoryName(pathAssembly);

            string zipFileName = Path.Combine(folderAssembly, "TestData", saveFile + ".zip");
            ZipFile.ExtractToDirectory(zipFileName, this._testDir.FullName);

            return new Instance
            {
                Key = "test-server-" + saveFile,
                LocalPath = Path.Combine(this._testDir.FullName, saveFile),
                TargetMajorVersion = 17,
                TargetMinorVersion = null
            };
        }

        protected Instance AddBlankDir(string dirName)
        {
            DirectoryInfo d = this._testDir.CreateSubdirectory(dirName);
            return new Instance
            {
                Key = "test-server-" + dirName,
                LocalPath = d.FullName,
                TargetMajorVersion = 17,
                TargetMinorVersion = null
            };
        }
    }
}
