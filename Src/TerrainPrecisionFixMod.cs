using System;
using HarmonyLib;
using UnityEngine;

namespace com.github.lhervier.ksp.terrainprecisionfix
{
    /// <summary>
    /// Places the terrain and the statics the same way at every load, where their own double precision
    /// coordinates say they are: installs each fix the settings leave on, and drives the statics fix
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

            GameEvents.onGameSceneLoadRequested.Add(OnGameSceneLoadRequested);
            GameEvents.onFlightReady.Add(OnFlightReady);
        }

        private void OnDestroy()
        {
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
    }
}
