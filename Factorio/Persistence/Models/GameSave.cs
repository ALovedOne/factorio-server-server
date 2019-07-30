using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Factorio.Persistence.Models
{
    public class GameSave
    {
        public int MajorVersion { get; }
        public int MinorVersion { get; }
        public int PatchVersion { get; }

        public IList<Mod> Mods { get; }

        public GameSave(int MajorVersion, int MinorVersion, int PatchVersion, IList<Mod> mods)
        {
            this.MajorVersion = MajorVersion;
            this.MinorVersion = MinorVersion;
            this.PatchVersion = PatchVersion;
            this.Mods = mods;
        }
    }
}
