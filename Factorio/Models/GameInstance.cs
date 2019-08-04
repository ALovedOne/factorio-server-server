using System.Collections.Generic;

namespace Factorio.Models
{
    public class GameInstance
    {
        public string Key { get; set; }

        // From server-info.json
        public string Name { get; set; }
        public string Description { get; set; }
        public FuzzyVersion TargetVersion { get; set; }
        
        // From mods directory
        public IEnumerable<Mod> Mods { get; set; }

        // From saves directory
        public GameSave LastSave { get; set; }
        // TODO:
        //  - preview.jpg

        public IReadOnlyDictionary<string, string> ImplementationInfo { get; set; }


        public ExecutionInfo CurrentExecution { get; set; }

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
                if (this.LastSave != null && this.TargetVersion <= this.LastSave.Version) return false;
                return true;
            }

        }
    }

    public class ExecutionInfo
    {
        public string Key { get; set; }
        public int Port { get; set; }
        public string Hostname { get; set; }
        public string InstanceKey { get; set; }
        public string RunningVersion { get; set; }
        public ExecutionStatus Status { get; set; }
    }

    public enum ExecutionStatus
    {
        Creating,
        Created,
        Starting,
        Started,
        Stopping,
        Stoppped,
        Error
    }

    public class GameSave
    {
        public SpecificVersion Version { get; }

        public IList<Mod> Mods { get; }

        public GameSave(int MajorVersion, int MinorVersion, int PatchVersion, IList<Mod> mods = null)
        {
            this.Version = new SpecificVersion(MajorVersion, MinorVersion, PatchVersion);
            this.Mods = mods ?? new List<Mod>();
        }
    }

    public class Mod
    {
        public string Name { get; set; }
        public SpecificVersion Version { get; set; }
    }

    #region Versions
    public class FuzzyVersion : Version
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int? Patch { get; set; }

        protected override int _Major => Major;
        protected override int _Minor => Minor;
        protected override int? _Patch => Patch;
    }

    public class SpecificVersion : Version
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }

        public SpecificVersion() { }

        public SpecificVersion(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        protected override int _Major => Major;
        protected override int _Minor => Minor;
        protected override int? _Patch => Patch;
    }

    public abstract class Version
    {
        protected abstract int _Major { get; }
        protected abstract int _Minor { get; }
        protected abstract int? _Patch { get; }

        public static bool operator <=(Version a, Version b)
        {
            if (a._Major < b._Major) return true;
            if (a._Major > b._Major) return false;

            if (a._Minor < b._Minor) return true;
            if (a._Minor > b._Minor) return false;

            if (a._Patch == null) return false;
            if (b._Patch == null) return true;

            return a._Patch <= b._Patch;
        }

        public static bool operator >=(Version a, Version b)
        {
            if (a._Major < b._Major) return false;
            if (a._Major > b._Major) return true;

            if (a._Minor < b._Minor) return false;
            if (a._Minor > b._Minor) return true;

            if (a._Patch == null) return true;
            if (b._Patch == null) return false;

            return a._Patch >= b._Patch;
        }
    }
    #endregion
}