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
        public static void PressQToExit(int exitCode)
        {
            //TODO: check the exit code
            Console.WriteLine("Press Q to exit...");
            do
            {
                while (!Console.KeyAvailable)
                {
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Q);
            System.Environment.Exit(exitCode);
        }

        static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
                builder.SetMinimumLevel(LogLevel.Debug)
                       .AddConsole()
                       .AddDebug()
            );
            var plogger = loggerFactory.CreateLogger<Program>();

            InputParser parser = new InputParser(loggerFactory.CreateLogger<InputParser>());
            ManagerInput mInput;
            try
            {
                mInput = parser.parseInput(args);
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
