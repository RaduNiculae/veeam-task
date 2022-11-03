using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veeam_task
{
    class InputParser
    {
        private readonly ILogger<InputParser> _logger;
        private const uint processNamePos = 0;
        private const uint maxLifetimePos = 1;
        private const uint monitorFreqPos = 2;
        private const uint argumentCount = 3;

        public InputParser(ILogger<InputParser> logger) =>
            _logger = logger;

        public ManagerInput parseInput(string[] args)
        {
            if (args.Length != argumentCount)
            {
                _logger.LogError("Incorect number of arguments!");
                throw new Exception();
            }

            string processName = args[processNamePos];
            int maxLifetime;
            int monitorFreq;
            try
            {
                maxLifetime = int.Parse(args[maxLifetimePos]);
                monitorFreq = int.Parse(args[monitorFreqPos]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

            if (maxLifetime < 0)
            {
                _logger.LogError("Maximum lifetime cannot be lower than 0!");
                throw new Exception();
            }

            if (monitorFreq <= 0)
            {
                _logger.LogError("Monitoring frequency cannot be 0 or lower!");
                throw new Exception();
            }
            
            return new ManagerInput(processName, maxLifetime, monitorFreq);
        }
    }
}
