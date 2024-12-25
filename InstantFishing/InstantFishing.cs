using System;
using System.Linq;
using BepInEx;
using HarmonyLib;
using InstantFishing.Config;

namespace InstantFishing
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.instantfishing";
        internal const string Name = "Instant Fishing";
        internal const string Version = "2.0.0.0";
        internal const string ModOptionsGuid = "evilmask.elinplugins.modoptions";
        internal const string ModOptionsAssemblyName = "ModOptions";
    }

    [BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
    internal class InstantFishing : BaseUnityPlugin
    {
        internal static InstantFishing Instance { get; private set; }
        
        private void Start()
        {
            Instance = this;
            
            InstantFishingConfig.LoadConfig(config: Config);
            
            Harmony.CreateAndPatchAll(type: typeof(Patcher), harmonyInstanceId: null);
            
            if (IsModOptionsInstalled())
            {
                try
                {
                    UI.UIController.RegisterUI();
                }
                catch (Exception ex)
                {
                    Log(payload: $"An error occurred during UI registration: {ex.Message}");
                }
            }
            else
            {
                Log(payload: "Mod Options is not installed. Skipping UI registration.");
            }
        }
        
        public static void Log(object payload)
        {
            Instance.Logger.LogInfo(data: payload);
        }
        
        private bool IsModOptionsInstalled()
        {
            try
            {
                return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Any(predicate: assembly => assembly.GetName().Name == ModInfo.ModOptionsAssemblyName);
            }
            catch (Exception ex)
            {
                Log(payload: $"Error while checking for Mod Options: {ex.Message}");
                return false;
            }
        }
    }
}