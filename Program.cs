using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;

namespace veeam_task
{
    class Program
    {
        private static int KEY_CHECK_MILLIS = 300;

        public static void PressQToExit(int exitCode)
        {
            Console.WriteLine("Press the Q key to exit.");
            do
            {
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(KEY_CHECK_MILLIS);
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Q);
            System.Environment.Exit(exitCode);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Program has started...");

            var loggerFactory = LoggerFactory.Create(builder =>
                builder.SetMinimumLevel(LogLevel.Debug)
                       .AddConsole()
                       .AddDebug()
            );
            var managerInputLogger = loggerFactory.CreateLogger<ManagerInput>();

            ManagerInput mInput;
            try
            {
                mInput = ManagerInput.parseInput(args, managerInputLogger);
            }
            catch
            {
                PressQToExit(1);
                return;
            }

            ProcessLifetimeManager manager = new ProcessLifetimeManager(
                mInput,
                loggerFactory.CreateLogger<ProcessLifetimeManager>()
            );

            manager.manageLifetime();
            PressQToExit(1);
        }
    }
}
