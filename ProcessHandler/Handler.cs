using System.Diagnostics;

namespace ProcessHandler
{
    internal class Handler
    {
        DateTime StartTime { get; set; }
        TimeSpan AllottedTime { get; init; }

        TimeSpan WaitInterval { get; init; }

        Process Process { get; init; }


        public Handler(Process process, TimeSpan allottedTime, TimeSpan waitInterval)
        {
            Process = process;
            AllottedTime = allottedTime;
            WaitInterval = waitInterval;
            InitStartTime();
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

        public async Task HandleAsync()
        {
            while (AllottedTimeEnded)
            {
                await Task.Delay(WaitInterval);
                if (Process.HasExited)
                    throw new InvalidOperationException($"Process {Process.ProcessName} has already been closed");
            }
            KillProcesses();
        }

        bool AllottedTimeEnded => (DateTime.Now - StartTime - AllottedTime).Ticks <= 0;
    }
}
