﻿using System.Diagnostics;

namespace ProcessHandler
{
    internal class Handler
    {
        DateTime StartTime { get; set; }
        TimeSpan AllottedTime { get; init; }

        TimeSpan WaitInterval { get; init; }

        Process Process { get; init; }

        AutoResetEvent Timer { get; } = new AutoResetEvent(true);

        public Handler(Process process, TimeSpan allottedTime, TimeSpan waitInterval)
        {
            Process = process;
            AllottedTime = allottedTime;
            WaitInterval = waitInterval;
            InitStartTime();
            Run();
        }

        void InitStartTime()
        {
            try
            {
                StartTime = Process.StartTime;
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                throw new InvalidProgramException($"You cannot work with the {Process.ProcessName} process. {e.Message}", e);
            }
            catch (NotSupportedException e)
            {
                throw new InvalidDataException($"The {Process.ProcessName} process is not running on this device", e);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException($"{Process.ProcessName} process is not running now", e);
            }
        }

        void KillProcesses()
        {
            try
            {
                Process.Kill();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            Trace.WriteLine($"{Process.ProcessName} was killed at {DateTime.Now.TimeOfDay}");
        }

        void Handle()
        {
            while (!Timer.WaitOne(WaitInterval) || (DateTime.Now - StartTime - AllottedTime).Ticks <= 0)
            {
                if (Process.HasExited)
                    throw new InvalidOperationException($"Process {Process.ProcessName} has already been closed");
            }
            KillProcesses();
        }

        void Run()
        {
            var thread = new Thread(Handle);
            thread.IsBackground = false;
            thread.Start();
        }


        public static Process[] GetProcesses(string name)
        {
            var processes = Process.GetProcessesByName(name);
            if (processes.Length == 0)
                throw new InvalidDataException($"Process {name} is not running");
            return processes;
        }
    }
}
