using Factorio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Factorio.Test
{
    public class TestWithTempDir : IDisposable
    {
        private readonly DirectoryInfo _testDir;

        public TestWithTempDir()
        {
            DirectoryInfo userTempDir = new DirectoryInfo(Path.GetTempPath());
            string randomName = Path.GetRandomFileName();

            _testDir = userTempDir.CreateSubdirectory(randomName);
        }

        public void Dispose()
        {
            if (_testDir.Exists)
            {
                _testDir.Delete(true);
            }
        }

        protected string FullPath { get { return _testDir.FullName; } }

        protected void AddTestSave(string saveFile, int? patchVersion = 50)
        {
            string pathAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string folderAssembly = Path.GetDirectoryName(pathAssembly);

            string zipFileName = Path.Combine(folderAssembly, "TestData", saveFile + ".zip");
            ZipFile.ExtractToDirectory(zipFileName, _testDir.FullName);
        }

        protected void AddBlankDir(string dirName, int? PatchVersion = null)
        {
            DirectoryInfo d = _testDir.CreateSubdirectory(dirName);
        }
    }
}
