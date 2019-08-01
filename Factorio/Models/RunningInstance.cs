using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Factorio.Models
{
    public class RunningInstance
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
}
