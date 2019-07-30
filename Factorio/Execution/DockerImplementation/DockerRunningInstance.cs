using Factorio.Execution.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Factorio.Execution
{
    public class DockerRunningInstance : IRunningInstance
    {

        public int Port { get; set; }
        public string Hostname { get; set; }
        public string InstanceKey { get; set; }
        public string Version { get; set; }

        public string ContainerID { get; set; }

    }
}
