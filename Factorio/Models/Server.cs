using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace factorio.Models
{
    public class Server
    {
        public string Slug { get; set; }

        // From server-info.json
        public string Name { get; set; }
        public string Description { get; set; }
        public int TargetMajorVersion { get; set; }
        public int? TargetMinorVersion { get; set; } // Minor version null => latest

        // From mods directory
        public IEnumerable<Mod> Mods { get; set; }

        // From saves directory
        public int? LastSaveMajorVersion { get; set; }
        public int? LastSaveMinorVersion { get; set; }
        // TODO:
        //  - preview.jpg

        // From Docker
        public int? Port { get; set; }
    }
}