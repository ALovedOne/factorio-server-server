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


        public bool Valid
        {
            get
            {
                if (!VersionValid) return false;
                // TODO - check mod versions
                return true;
            }
        }

        private bool VersionValid
        {
            get
            {
                if (this.LastSave == null) return true;

                if (this.TargetMajorVersion > this.LastSave.MajorVersion) return true;
                if (this.TargetMajorVersion < this.LastSave.MajorVersion) return false;

                if (this.TargetMinorVersion > this.LastSave.MinorVersion) return true;
                if (this.TargetMinorVersion < this.LastSave.MinorVersion) return false;

                if (this.TargetPatchVersion == null) return true;
                if (this.TargetPatchVersion >= this.LastSave.PatchVersion) return true;
                return false;
            }
        }
    }
}