using Factorio.Persistence.Interfaces;
using System.Collections.Generic;

namespace Factorio.Persistence.Models
{
    public class Instance : IInstance
    {
        public string Key { get; set; }

        // From server-info.json
        public string Name { get; set; }
        public string Description { get; set; }
        
        public int TargetMajorVersion { get; set; }
        public int TargetMinorVersion { get; set; }
        public int? TargetPatchVersion { get; set; }

        // From mods directory
        public IEnumerable<Mod> Mods { get; set; }

        public GameSave LastSave { get; set; }

        public string LocalPath { get; set; }
    }
}