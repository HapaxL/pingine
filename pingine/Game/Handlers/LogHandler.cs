using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace pingine.Game.Handlers
{
    public class LogHandler
    {
        private void LogDebug(string line)
        {
            Debug.WriteLine(line);
        }

        public void LogDebugInfo(string source, string log)
        {
            if (!string.IsNullOrWhiteSpace(log))
            {
                var str = $"DEBUG INFO {source}: {log}";
                LogDebug(str);
                Console.WriteLine(str);
            }
        }

        public void LogDebugWarning(string source, string log)
        {
            if (!string.IsNullOrWhiteSpace(log))
            {
                var str = $"WARNING {source}: {log}";
                LogDebug(str);
                Console.WriteLine(str);
            }
        }

        public void LogDebugError(string source, string log)
        {
            if (!string.IsNullOrWhiteSpace(log))
            {
                var str = $"ERROR {source}: {log}";
                LogDebug(str);
                Console.WriteLine(str);
            }
        }

        public void LogGLError(string source, ErrorCode code)
        {
            if (code != ErrorCode.NoError)
            {
                var str = $"GL ERROR {source}: {code}";
                LogDebug(str);
                Console.WriteLine(str);
            }
        }
    }
}
