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
        /// Whether terrain scatter (rocks, grass, trees) is drawn from its terrain quads. Off by default: it
        /// moves stock objects away from where other mods may look for them.
        /// </summary>
        public static bool FixScatter { get; private set; } = false;

        /// <summary>
        /// Whether the collider of the stock ground anchor reaches down to the bottom of the anchor, so that a
        /// load puts it back at the height it was placed at.
        /// </summary>
        public static bool FixGroundAnchorModel { get; private set; } = true;

        /// <summary>
        /// Whether a vessel holding a stock ground anchor is loaded where it was saved, instead of being raised
        /// to the height the terrain is computed at.
        /// </summary>
        public static bool FixGroundAnchorLoad { get; private set; } = true;

        /// <summary>
        /// Whether Kopernicus, when installed, is patched to cope with statics out of their sphere. Only
        /// meaningful with the statics fix on, which then breaks Kopernicus' flag fix when this is off.
        /// </summary>
        public static bool PatchKopernicus { get; private set; } = true;

        /// <summary>
        /// Whether Kerbal Konstructs, when installed, is patched to cope with statics out of their sphere.
        /// Only meaningful with the statics fix on, which then breaks its group editor when this is off.
        /// </summary>
        public static bool PatchKerbalKonstructs { get; private set; } = true;

        /// <summary>
        /// Reads the settings file and applies its log level. A missing file, a missing value or an unknown
        /// value leaves the default: the terrain, the statics and the ground anchor fixed, the scatter left as
        /// stock, both mods patched, logging at Info.
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
            FixScatter = ReadSwitch(node, "fixScatter", FixScatter, path);
            FixGroundAnchorModel = ReadSwitch(node, "fixGroundAnchorModel", FixGroundAnchorModel, path);
            FixGroundAnchorLoad = ReadSwitch(node, "fixGroundAnchorLoad", FixGroundAnchorLoad, path);
            PatchKopernicus = ReadSwitch(node, "patchKopernicus", PatchKopernicus, path);
            PatchKerbalKonstructs = ReadSwitch(node, "patchKerbalKonstructs", PatchKerbalKonstructs, path);
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
