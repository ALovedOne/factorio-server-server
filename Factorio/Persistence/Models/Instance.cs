using System.Collections.Generic;

namespace Factorio.Persistence.Models
{
    public class Instance : IInstance
    {
        public string Slug { get; set; }

        // From server-info.json
        public string Name { get; set; }
        public string Description { get; set; }
        public int TargetMajorVersion { get; set; }
        public int? TargetMinorVersion { get; set; }

        // From mods directory
        public IEnumerable<Mod> Mods { get; set; }

        // From saves directory
        public int? LastSaveMajorVersion { get; set; }
        public int? LastSaveMinorVersion { get; set; }
        // TODO:
        //  - preview.jpg
    }
}