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
        int? TargetMinorVersion { get; set; }

        // From mods directory
        IEnumerable<IMod> Mods { get; set; }

        // From saves directory
        int? LastSaveMajorVersion { get; set; }
        int? LastSaveMinorVersion { get; set; }
        // TODO:
        //  - preview.jpg
    }

    public interface IMod
    {
        string Name { get; set; }
    }
}