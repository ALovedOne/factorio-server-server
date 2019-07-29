using System.Collections.Generic;

namespace Factorio.Persistence.Models
{
    public interface IInstance
    {
        string Slug { get; set; }

        // From server-info.json
        string Name { get; set; }
        string Description { get; set; }
        int TargetMajorVersion { get; set; }
        int? TargetMinorVersion { get; set; }

        // From mods directory
        IEnumerable<Mod> Mods { get; set; }

        // From saves directory
        int? LastSaveMajorVersion { get; set; }
        int? LastSaveMinorVersion { get; set; }
        // TODO:
        //  - preview.jpg
    }
}