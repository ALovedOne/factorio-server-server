using Factorio.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Factorio.Persistence.Models
{
    public class Mod 
    {
        private readonly string _name;
        private readonly byte[] _version;

        public Mod(string Name, byte[] modVersion)
        {
            this._name = Name;
            this._version = modVersion;
        }

        public string Name => this._name;
        public int Major => this._version[0];
        public int Minor => this._version[1];
        public int Patch => this._version[2];
    }
}
