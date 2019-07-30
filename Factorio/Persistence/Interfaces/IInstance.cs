using Factorio.Persistence.Models;
using System.Collections.Generic;

namespace Factorio.Persistence.Interfaces
{
    public interface IInstance
    {
        string Key { get; set; }

        // From server-info.json
        string Name { get; set; }
        string Description { get; set; }
        int TargetMajorVersion { get; set; }
        int TargetMinorVersion { get; set; }
        int? TargetPatchVersion { get; set; }


        // From mods directory
        IEnumerable<Mod> Mods { get; set; }

        // From saves directory
        GameSave LastSave { get; set; }
        // TODO:
        //  - preview.jpg
    }
}