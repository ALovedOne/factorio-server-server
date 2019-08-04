using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Factorio.Services.Execution.DockerImplementation
{
    public class DockerExecutionOptions
    {
        public int PortRangeBegin { get; set; }
        public int PortRangeEnd { get; set; }
        public string DockerUrl { get; set; }
    }
}
