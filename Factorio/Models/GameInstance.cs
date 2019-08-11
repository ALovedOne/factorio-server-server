using System;
using System.Collections.Generic;
using System.Linq;

namespace Factorio.Models
{
    public class GameInstance
    {
        public GameInstance()
        {
            TargetVersion = new FuzzyVersion();
            Mods = new List<TargetMod>();
        }

        public string Key { get; set; }

        // From server-info.json
        public string Name { get; set; }
        public string Description { get; set; }
        public FuzzyVersion TargetVersion { get; set; }

        // From mods directory, config
        public IEnumerable<TargetMod> Mods { get; set; }

        // From saves directory
        public GameSave LastSave { get; set; }

        public IReadOnlyDictionary<string, string> ImplementationInfo { get; set; }
        public ExecutionInfo CurrentExecution { get; set; }

        public bool Valid
        {
            get
            {
                if (!VersionValid) return false;
                if (!ModsValid) return false;
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

        private bool ModsValid
        {
            get
            {
                if (this.LastSave == null) return true;

                IDictionary<string, TargetMod> desiredMods = this.Mods.ToDictionary(m => m.Name);
                IDictionary<string, SpecificMod> installedMods = this.LastSave.Mods.ToDictionary(m => m.Name);

                // Can't remove mods
                if (installedMods.Keys.Except(desiredMods.Keys).Any()) return false;

                // Can't downgrade mods
                if (installedMods.All(installed => desiredMods.TryGetValue(installed.Key, out TargetMod desired) && installed.Value.Version <= desired.TargetVersion)) return false;

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

        public IList<SpecificMod> Mods { get; }

        public GameSave(int MajorVersion, int MinorVersion, int PatchVersion, IList<SpecificMod> mods = null)
        {
            this.Version = new SpecificVersion(MajorVersion, MinorVersion, PatchVersion);
            this.Mods = mods ?? new List<SpecificMod>();
        }
    }

    public class TargetMod
    {
        public string Name { get; set; }
        public FuzzyVersion TargetVersion { get; set; }
    }

    public class SpecificMod
    {
        public string Name { get; set; }
        public SpecificVersion Version { get; set; }

        public static implicit operator TargetMod(SpecificMod m) => new TargetMod { Name = m.Name, TargetVersion = m.Version };
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

        public static implicit operator FuzzyVersion(SpecificVersion v) => new FuzzyVersion { Major = v.Major, Minor = v.Minor, Patch = v.Patch };
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