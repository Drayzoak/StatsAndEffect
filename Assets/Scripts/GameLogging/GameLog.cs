using UberLogger;

namespace GameLogging
{
    public static class GameLog
    {
        public static bool Enabled = true;

        public static void Info(string category, string message)
        {
            if (!Enabled) return;

            UberDebug.LogChannel(category, message);
        }

        public static void Warn(string category, string message)
        {
            if (!Enabled) return;

            UberDebug.LogWarningChannel(category, message);
        }

        public static void Error(string category, string message)
        {
            if (!Enabled) return;

            UberDebug.LogErrorChannel(category, message);
        }
    }
}