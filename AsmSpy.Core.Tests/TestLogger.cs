using System;

namespace AsmSpy.Core.Tests
{
    public class TestLogger : ILogger
    {
        public void LogError(string message)
        {
            Console.WriteLine($"[ERROR] {message}");
        }

        public void LogMessage(string message)
        {
            Console.WriteLine($"[LOG] {message}");
        }

        public void LogWarning(string message)
        {
            Console.WriteLine($"[WARNING] {message}");
        }
    }
}
