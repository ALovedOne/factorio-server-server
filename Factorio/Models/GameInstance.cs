using System.Collections.Generic;

namespace Factorio.Models
{
    public class GameInstance
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

        // From saves directory
        public GameSave LastSave { get; set; }
        // TODO:
        //  - preview.jpg

        public IReadOnlyDictionary<string, string> ImplementationInfo { get; set; }
    }
}