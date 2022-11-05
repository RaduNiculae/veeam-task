using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veeam_task
{
    public class ManagerInput
    {
        private string processName;
        private int maxLifetime;
        private int monitorFreq;
        public string ProcessName => processName;
        public int MaxLifetime => maxLifetime;
        public int MonitorFreq => monitorFreq;

        private const int PROCESS_NAME_POS = 0;
        private const int MAX_LIFETIME_POS = 1;
        private const int MONITOR_FREQ_POS = 2;
        private const int ARGUMENT_COUNT = 3;
        private const int MIN_TO_MILLISECONDS_MUL = 60000;
        private const string INVALID_NUM_ARGS = "Incorect number of arguments!";
        private const string INVALID_PROCESS_NAME = "Invalid process name!";
        private const string INVALID_MAX_LIFETIME = "Maximum lifetime cannot be lower than 0!";
        private const string INVALID_MONITOR_FREQ = "Monitoring frequency cannot be 0 or lower!";

        private ManagerInput(string processName, int maxLifetime, int monitorFreq)
        {
            this.processName = processName;
            this.maxLifetime = maxLifetime;
            this.monitorFreq = monitorFreq;
        }
        
        public static ManagerInput parseInput(string[] args, ILogger<ManagerInput> _logger)
        {
            if (args.Length != ARGUMENT_COUNT)
            {
                _logger.LogError(INVALID_NUM_ARGS);
                throw new Exception(INVALID_NUM_ARGS);
            }

            // Check for invalid process name
            string processName = args[PROCESS_NAME_POS];
            if (string.IsNullOrEmpty(processName) ||
                processName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
            {
                _logger.LogError(INVALID_PROCESS_NAME);
                throw new Exception(INVALID_PROCESS_NAME);
            }

            int maxLifetime;
            int monitorFreq;
            try
            {
                maxLifetime = int.Parse(args[MAX_LIFETIME_POS]);
                monitorFreq = int.Parse(args[MONITOR_FREQ_POS]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }

            if (maxLifetime < 0)
            {
                _logger.LogError(INVALID_MAX_LIFETIME);
                throw new Exception(INVALID_MAX_LIFETIME);
            }

            if (monitorFreq <= 0)
            {
                _logger.LogError(INVALID_MONITOR_FREQ);
                throw new Exception(INVALID_MONITOR_FREQ);
            }

            /* Check for overflow when transforming to miliseconds.
             * The Timer accepts values of Int32.MaxValue so converting to long would not help.
             */
            try
            {
                checked
                {
                    monitorFreq = monitorFreq * MIN_TO_MILLISECONDS_MUL;
                }
            }
            catch (OverflowException ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }

            return new ManagerInput(processName, maxLifetime, monitorFreq);
        }
    
    }
}
