using System;

namespace MalikP.GitHub.LabelSynchronizer.Loggers
{
    public class ConsoleLogger : IConsoleLogger
    {
        public void WriteLog(string message, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
