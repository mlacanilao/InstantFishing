using BepInEx;
using HarmonyLib;

namespace InstantFishing
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.instantfishing";
        internal const string Name = "Instant Fishing";
        internal const string Version = "1.4.0.0";
    }

    [BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
    public class InstantFishing : BaseUnityPlugin
    {
        private void Start()
        {
            InstantFishingConfig.LoadConfig(Config);
            Harmony.CreateAndPatchAll(type: typeof(Patcher), harmonyInstanceId: null);
        }
    }
}
