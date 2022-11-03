using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veeam_task
{
    class ManagerInput
    {
        private string processName;
        private int maxLifetime;
        private int monitorFreq;
        public string ProcessName => processName;
        public int MaxLifetime => maxLifetime;
        public int MonitorFreq => monitorFreq;

        public ManagerInput(string processName, int maxLifetime, int monitorFreq)
        {
            this.processName = processName;
            this.maxLifetime = maxLifetime;
            this.monitorFreq = monitorFreq;
        }
    }
}
