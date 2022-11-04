using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veeam_task
{
    public class InputParser
    {
        private readonly ILogger<InputParser> _logger;
        private const uint PROCESS_NAME_POS = 0;
        private const uint MAX_LIFETIME_POS = 1;
        private const uint MONITOR_FREQ_POS = 2;
        private const uint ARGUMENT_COUNT = 3;
        private const int MIN_TO_MILLISECONDS_MUL = 60000;

        public InputParser(ILogger<InputParser> logger) =>
            _logger = logger;

        public ManagerInput parseInput(string[] args)
        {
            if (args.Length != ARGUMENT_COUNT)
            {
                _logger.LogError("Incorect number of arguments!");
                throw new Exception("Incorect number of arguments!");
            }

            // TODO: check for invalid process name
            string processName = args[PROCESS_NAME_POS];
            int maxLifetime;
            int monitorFreq;
            try
            {
                maxLifetime = int.Parse(args[MAX_LIFETIME_POS]);
                monitorFreq = int.Parse(args[MONITOR_FREQ_POS]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

            if (maxLifetime < 0)
            {
                _logger.LogError("Maximum lifetime cannot be lower than 0!");
                throw new Exception("Maximum lifetime cannot be lower than 0!");
            }

            if (monitorFreq <= 0)
            {
                _logger.LogError("Monitoring frequency cannot be 0 or lower!");
                throw new Exception("Monitoring frequency cannot be 0 or lower!");
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
                _logger.LogError(ex.Message);
                throw;
            }

            return new ManagerInput(processName, maxLifetime, monitorFreq);
        }
    }
}
