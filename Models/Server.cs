using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace factorio.Models
{
    public class Server
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MajorVersion { get; set; }
        public int? MinorVersion { get; set; } // Minor version null => latest
        public IEnumerable<Mod> Mods { get; set; }
        public int? Port { get; set; }
    }
}
