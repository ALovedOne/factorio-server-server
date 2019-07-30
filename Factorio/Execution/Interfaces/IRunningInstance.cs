using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Factorio.Execution
{
    public interface IRunningInstance
    {
        int Port { get; set; }
        string Hostname { get; set; }

        string InstanceKey { get; set; }
        string Version { get; set; }

    }
}
