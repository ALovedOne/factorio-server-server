using Factorio.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Factorio.Persistence.Utils
{
    public class SaveFolderParser
    {
        public static GameSave ParserZipFile(FileInfo fileName)
        {
            int majorVersion, minorVersion, patchVersion;
            List<SpecificMod> modList = new List<SpecificMod>();

            using (ZipArchive zip = ZipFile.OpenRead(fileName.FullName))
            {
                ZipArchiveEntry mapDATfile = zip.Entries.FirstOrDefault(e => e.FullName.EndsWith("level.dat"));
                if (mapDATfile == null) return null;

                using (BinaryReader r = new BinaryReader(mapDATfile.Open()))
                {
                    // Map version = <major>.<minor>.<patch>-<subversion>
                    majorVersion = r.ReadInt16();
                    minorVersion = r.ReadInt16();
                    patchVersion = r.ReadInt16();
                    r.ReadInt16(); // Subversion

                    if (minorVersion == 17)
                    {
                        r.ReadInt16(); // Unknown 2 bytes, difficulty?
                        r.ReadString();

                        SpecificMod baseMod = ReadMod17(r);

                        r.ReadBytes(7);

                        int modCnt = r.ReadByte();
                        for (int i = 0; i < modCnt; i++)
                        {
                            modList.Add(ReadMod17(r));
                        }
                    }
                }
            }

            return new GameSave(majorVersion, minorVersion, patchVersion, modList);
        }

        private static SpecificMod ReadMod17(BinaryReader r)
        {
            string name = r.ReadString();
            byte[] version = r.ReadBytes(3);
            r.ReadBytes(4);
            return new SpecificMod
            {
                Name = name,
                Version = new SpecificVersion(version[0], version[1], version[2])
            };
        }
    }
}
