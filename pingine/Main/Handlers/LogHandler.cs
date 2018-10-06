using System;
using System.Diagnostics;

namespace pingine.Main.Handlers
{
    public static class LogHandler
    {
        public static void LogDebug(string line)
        {
            Debug.WriteLine(line);
        }

        public static void LogDebugInfo(string source, string log)
        {
            if (!string.IsNullOrWhiteSpace(log))
            {
                LogDebug($"DEBUG INFO {source}: {log}");
                Console.WriteLine(source + ": " + log);
            }
        }
    }
}
