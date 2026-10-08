using System;
using HarmonyLib;
using UnityEngine;

namespace com.github.lhervier.ksp.terrainprecisionfix
{
    /// <summary>
    /// Places the terrain and the statics, and, when asked to, the terrain scatter, the same way at every
    /// load, where their own double precision coordinates say they are, and keeps the stock ground anchor
    /// at the height it was placed at: installs each fix the settings leave on, and drives the statics fix
    /// through scene changes and from frame to frame.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class TerrainPrecisionFixMod : MonoBehaviour
    {
        private const string HarmonyId = "com.github.lhervier.ksp.terrainprecisionfix";

        private void Start()
        {
            // An addon started once is still destroyed at the first scene change; the statics fix needs
            // this one for the whole session.
            DontDestroyOnLoad(gameObject);

            Settings.Load();
            Log.Info($"Version {typeof(TerrainPrecisionFixMod).Assembly.GetName().Version}, log level {Log.Level}");
            Harmony harmony = new Harmony(HarmonyId);

            if (!Settings.FixTerrain)
            {
                Log.Info("Terrain fix turned off in the settings");
            }
            else
            {
                try
                {
                    TerrainFix.Install(harmony);
                    Log.Info("Terrain fix installed");
                }
                catch (Exception e)
                {
                    // Whatever patch did get applied does nothing: the terrain is exactly stock.
                    Log.Error($"Could not install the terrain fix, the terrain is left as stock builds it: {e}");
                }
            }

            if (!Settings.FixStatics)
            {
                Log.Info("Statics fix turned off in the settings");
            }
            else
            {
                try
                {
                    StaticsFix.Install(harmony);
                    Log.Info("Statics fix installed");
                }
                catch (Exception e)
                {
                    // Whatever patch did get applied does nothing: the statics are exactly where stock puts
                    // them.
                    Log.Error($"Could not install the statics fix, the statics are left where stock places them: {e}");
                }
            }

            if (!Settings.FixScatter)
            {
                Log.Info("Scatter fix turned off in the settings");
            }
            else
            {
                try
                {
                    if (ScatterFix.Install(harmony))
                    {
                        Log.Info("Scatter fix installed");
                    }
                    else
                    {
                        Log.Warning("Scatter fix not installed: Rock Precision Fix is installed, and does the same."
                            + " Remove Rock Precision Fix to use this mod's scatter fix instead");
                    }
                }
                catch (Exception e)
                {
                    // Whatever patch did get applied does nothing: the scatter is exactly where stock puts it.
                    Log.Error($"Could not install the scatter fix, the scatter is left where stock places it: {e}");
                }
            }

            if (!Settings.FixGroundAnchorModel)
            {
                Log.Info("Ground anchor model fix turned off in the settings");
            }
            else
            {
                // The parts are not loaded yet: the anchor's collider is fixed once they are, and again if the
                // part database is ever reloaded.
                GameEvents.OnPartLoaderLoaded.Add(OnPartLoaderLoaded);
                Log.Info("Ground anchor model fix installed");
            }

            if (!Settings.FixGroundAnchorLoad)
            {
                Log.Info("Ground anchor load fix turned off in the settings");
            }
            else
            {
                try
                {
                    GroundAnchorFix.Install(harmony);
                    Log.Info("Ground anchor load fix installed");
                }
                catch (Exception e)
                {
                    // The patch is not in place: anchored vessels are loaded as stock loads them.
                    Log.Error($"Could not install the ground anchor load fix, anchored vessels are loaded as stock loads them: {e}");
                }
            }

            GameEvents.onGameSceneLoadRequested.Add(OnGameSceneLoadRequested);
            GameEvents.onFlightReady.Add(OnFlightReady);
        }

        private void OnDestroy()
        {
            GameEvents.OnPartLoaderLoaded.Remove(OnPartLoaderLoaded);
            GameEvents.onGameSceneLoadRequested.Remove(OnGameSceneLoadRequested);
            GameEvents.onFlightReady.Remove(OnFlightReady);
        }

        private void Update()
        {
            StaticsFix.Update();
        }

        // Instance methods, although they only reach static state: GameEvents throws on a static handler.
        private void OnGameSceneLoadRequested(GameScenes scene)
        {
            StaticsFix.OnSceneChangeRequested();
        }

        private void OnFlightReady()
        {
            StaticsFix.OnFlightReady();
        }

        private void OnPartLoaderLoaded()
        {
            try
            {
                GroundAnchorFix.Apply();
            }
            catch (Exception e)
            {
                // The collider is only replaced once the new one is complete: the anchor is exactly stock.
                Log.Error($"Could not fix the ground anchor's model, it is left as stock: {e}");
            }
        }
    }
}
