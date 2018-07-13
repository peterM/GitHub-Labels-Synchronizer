using System;

namespace MalikP.GitHub.LabelSynchronizer.Loggers
{
    public interface IConsoleLogger
    {
        void WriteLog(string message, ConsoleColor parameter);
    }
}
