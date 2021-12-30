// See https://aka.ms/new-console-template for more information
using ProcessHandler;
using System.Diagnostics;

System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

Console.WriteLine("Run!");
string logPath = "log.txt";
//file.Position = file.Length;
var listener = new TextWriterTraceListener(File.AppendText(logPath));
Trace.AutoFlush = true;
Trace.Listeners.Add(listener);

if (args.Length < 3)
    throw new ArgumentException("There were less than three arguments");
int allotted, period;
if (!int.TryParse(args[1], out allotted))
    throw new ArgumentException("The second argument was not an integer");
if (!int.TryParse(args[2], out period))
    throw new ArgumentException("The third argument was not an integer");
if (period < 0)
    throw new ArgumentException("The period must not be negative");

var processes = Handler.GetProcesses(args[0]);
var allottedTime = TimeSpan.FromMinutes(allotted);
var periodTime = TimeSpan.FromMinutes(period);

var handlers = new List<Handler>();

foreach (var process in processes)
    handlers.Add(new Handler(process, allottedTime, periodTime));


static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
{
    Console.WriteLine(((Exception)e.ExceptionObject).Message);
    Trace.WriteLine(((Exception)e.ExceptionObject).Message);
    if(e.IsTerminating)
        Environment.Exit(-1);
}