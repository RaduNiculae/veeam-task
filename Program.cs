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
    class ProcessLifetimeManager
    {
        private string processName;
        private ulong maxLifetime;
        private ulong monitorFreq;

        public ProcessLifetimeManager(string processName, ulong maxLifetime, ulong monitorFreq)
        {
            this.processName = processName;
            this.maxLifetime = maxLifetime;
            this.monitorFreq = monitorFreq;
        }

        public void manageLifetime()
        {
            Timer monitorTimer = new Timer(monitorFreq * 60 * 10);
            monitorTimer.Elapsed += checkProcessLifetime;
            monitorTimer.AutoReset = true;
            monitorTimer.Enabled = true;

            do
            {
                while (!Console.KeyAvailable)
                {
                    // Do something
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Q);
        }

        private void checkProcessLifetime(Object source, ElapsedEventArgs e)
        {
            Process[] managedProcesses = Process.GetProcessesByName(processName);

            foreach (var process in managedProcesses)
            {
                TimeSpan lifetime;
                try
                {
                    lifetime = DateTime.Now - process.StartTime;
                }
                catch (Win32Exception ex)
                {
                    if (ex.NativeErrorCode == 5)
                        continue;
                    throw;
                }

                if (lifetime.Minutes < 0)
                {
                    Console.WriteLine("Wrong time!");
                    return;
                }

                if (lifetime.TotalMinutes > maxLifetime)
                {
                    try
                    {
                        int pid = process.Id;
                        process.Kill();
                        Console.WriteLine("Process {0} with id {1} has been killed.", processName, pid);
                    }
                    catch (Exception killEx)
                    {
                        Console.WriteLine(killEx.Message);
                    }
                }
            }
        }
    }

    class InputParser
    {
        public ProcessLifetimeManager parseInput(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Incorect number of arguments!");
                throw new Exception("Incorect number of arguments!");
            }

            string processName = args[0];

            try
            {
                ulong maxLifetime = ulong.Parse(args[1]);
                ulong monitorFreq = ulong.Parse(args[2]);
                if (monitorFreq == 0)
                {
                    throw new Exception("Monitoring frequency cannot be 0!");
                }
                return new ProcessLifetimeManager(processName, maxLifetime, monitorFreq);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            InputParser parser = new InputParser();
            ProcessLifetimeManager manager;
            try
            {
                manager = parser.parseInput(args);
            }
            catch
            {
                Console.WriteLine("Input parsing failed!");
                return;
            }
            manager.manageLifetime();
        }
    }
}
