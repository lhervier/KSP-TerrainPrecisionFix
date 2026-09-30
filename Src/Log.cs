namespace com.github.lhervier.ksp.terrainprecisionfix
{
    /// <summary>How much the mod writes to KSP.log, from the quietest to the most talkative.</summary>
    internal enum LogLevel
    {
        Error,
        Warning,
        Info,
        Debug,
        Trace
    }

    /// <summary>
    /// Writes to KSP.log, tagged with the mod's name, filtered by the level set in
    /// PluginData/settings.cfg. The level is read once, when KSP starts.
    /// </summary>
    internal static class Log
    {
        private const string Prefix = "[TerrainPrecisionFix] ";

        private static LogLevel _level = LogLevel.Info;

        /// <summary>The level messages are filtered at; Info until the settings are read.</summary>
        public static LogLevel Level
        {
            get { return _level; }
            set { _level = value; }
        }

        public static bool IsDebugEnabled => _level >= LogLevel.Debug;
        public static bool IsTraceEnabled => _level >= LogLevel.Trace;

        public static void Error(string message)
        {
            UnityEngine.Debug.LogError(Prefix + message);
        }

        public static void Warning(string message)
        {
            if (_level >= LogLevel.Warning)
            {
                UnityEngine.Debug.LogWarning(Prefix + message);
            }
        }

        public static void Info(string message)
        {
            if (_level >= LogLevel.Info)
            {
                UnityEngine.Debug.Log(Prefix + message);
            }
        }

        public static void Debug(string message)
        {
            if (_level >= LogLevel.Debug)
            {
                UnityEngine.Debug.Log(Prefix + "[DEBUG] " + message);
            }
        }

        public static void Trace(string message)
        {
            if (_level >= LogLevel.Trace)
            {
                UnityEngine.Debug.Log(Prefix + "[TRACE] " + message);
            }
        }
    }
}
