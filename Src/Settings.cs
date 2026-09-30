using System;
using System.IO;

namespace com.github.lhervier.ksp.terrainprecisionfix
{
    /// <summary>
    /// What the player can set in PluginData/settings.cfg, next to the DLL. Read once, when KSP starts.
    /// </summary>
    internal static class Settings
    {
        /// <summary>Whether the terrain is placed in double precision.</summary>
        public static bool FixTerrain { get; private set; } = true;

        /// <summary>Whether the statics (PQSCity) are placed in double precision.</summary>
        public static bool FixStatics { get; private set; } = true;

        /// <summary>
        /// Reads the settings file and applies its log level. A missing file, a missing value or an unknown
        /// value leaves the default: everything fixed, logging at Info.
        /// </summary>
        public static void Load()
        {
            string folder = Path.GetDirectoryName(typeof(Settings).Assembly.Location);
            string path = Path.Combine(Path.Combine(folder, "PluginData"), "settings.cfg");
            if (!File.Exists(path))
            {
                Log.Warning($"No settings file at {path}, using the defaults");
                return;
            }
            ConfigNode node = ConfigNode.Load(path);
            if (node == null)
            {
                Log.Warning($"Could not read {path}, using the defaults");
                return;
            }

            string level = node.GetValue("logLevel");
            LogLevel parsed;
            if (Enum.TryParse(level, true, out parsed) && Enum.IsDefined(typeof(LogLevel), parsed))
            {
                Log.Level = parsed;
            }
            else
            {
                Log.Warning($"Unknown logLevel '{level}' in {path}, logging at {Log.Level}");
            }

            FixTerrain = ReadSwitch(node, "fixTerrain", FixTerrain, path);
            FixStatics = ReadSwitch(node, "fixStatics", FixStatics, path);
        }

        /// <summary>
        /// The value of an on/off setting, or <paramref name="defaultValue"/> when it is missing or
        /// unreadable.
        /// </summary>
        private static bool ReadSwitch(ConfigNode node, string name, bool defaultValue, string path)
        {
            string value = node.GetValue(name);
            if (value == null)
            {
                return defaultValue;
            }
            bool parsed;
            if (bool.TryParse(value, out parsed))
            {
                return parsed;
            }
            Log.Warning($"Unknown {name} '{value}' in {path}, using {defaultValue}");
            return defaultValue;
        }
    }
}
