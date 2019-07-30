using Factorio.Persistence.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Factorio.Persistence.Utils
{
    public class SaveFolderParser
    {
        public static GameSave ParserZipFile(FileInfo fileName)
        {
            int majorVersion, minorVersion, patchVersion;
            List<Mod> modList = new List<Mod>();

            using (ZipArchive zip = ZipFile.OpenRead(fileName.FullName))
            {
                ZipArchiveEntry mapDATfile = zip.Entries.FirstOrDefault(e => e.FullName.EndsWith("level.dat"));
                if (mapDATfile == null) return null;

                using (BinaryReader r = new BinaryReader(mapDATfile.Open()))
                {
                    // Map version = 0.<major>.<minor>-<subversion>
                    majorVersion = r.ReadInt16(); 
                    minorVersion = r.ReadInt16();
                    patchVersion = r.ReadInt16();
                    r.ReadInt16(); // Subversion

                    if (majorVersion == 17)
                    {
                        r.ReadInt16(); // Unknown 2 bytes, difficulty?
                        r.ReadString();

                        Mod baseMod = ReadMod17(r);

                        r.ReadBytes(7);

                        int modCnt = r.ReadByte();
                        for (int i = 0; i < modCnt; i++)
                        {
                            modList.Add(ReadMod17(r));
                        }
                    }
                }
            }

            return new GameSave(majorVersion, minorVersion,patchVersion, modList);
        }

        private static Mod ReadMod17(BinaryReader r)
        {
            string name = r.ReadString();
            byte[] version = r.ReadBytes(3);
            r.ReadBytes(4);
            return new Mod(name, version);
        }
    }
}
