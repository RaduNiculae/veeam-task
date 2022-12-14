using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace veeam_task
{
    public class ProcessLifetimeManager
    {
        private readonly ManagerInput mInput;
        private readonly ILogger<ProcessLifetimeManager> _logger;
        public ProcessLifetimeManager(ManagerInput mInput, ILogger<ProcessLifetimeManager> logger)
        {
            this.mInput = mInput;
            _logger = logger;
        }

        public void manageLifetime()
        {
            // No need for exception checking. Previous checks for strict posive and overflow.
            Timer monitorTimer = new Timer(mInput.MonitorFreq);
            monitorTimer.Elapsed += checkProcessLifetime;
            monitorTimer.AutoReset = true;
            monitorTimer.Enabled = true;

            Program.PressQToExit(0);
        }

        private void checkProcessLifetime(Object source, ElapsedEventArgs e)
        {
            Process[] managedProcesses = Process.GetProcessesByName(mInput.ProcessName);

            foreach (var process in managedProcesses)
            {
                TimeSpan lifetime;
                try
                {
                    lifetime = DateTime.Now - process.StartTime;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    continue;
                }

                if (lifetime.TotalMinutes > mInput.MaxLifetime)
                {
                    try
                    {
                        int pid = process.Id;
                        process.Kill();
                        _logger.LogInformation("Process {0} with id {1} has been killed.", mInput.ProcessName, pid);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex.Message);
                    }
                }
            }
        }

        public bool Equals(ProcessLifetimeManager manager)
        {
            if (manager is null)
            {
                return false;
            }

            if (ReferenceEquals(this, manager))
            {
                return true;
            }

            return (mInput.ProcessName == manager.mInput.ProcessName) &&
                   (mInput.MaxLifetime == manager.mInput.MaxLifetime) &&
                   (mInput.MonitorFreq == manager.mInput.MonitorFreq);
        }
    }
}
