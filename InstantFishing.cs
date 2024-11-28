using BepInEx;
using HarmonyLib;

namespace InstantFishing
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.instantfishing";
        internal const string Name = "Instant Fishing";
        internal const string Version = "1.1.0.0";
    }

    [BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
    internal partial class InstantFishing : BaseUnityPlugin
    {
        internal static InstantFishing Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            InstantFishingConfig.LoadConfig(Config);
            var harmony = new Harmony(ModInfo.Guid);
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(AI_Fish.ProgressFish), nameof(AI_Fish.ProgressFish.OnProgress))]
    internal static class InstantFishingPatch
    {
        [HarmonyPrefix]
        public static void Prefix(AI_Fish.ProgressFish __instance)
        {
            if (InstantFishingConfig.EnableInstantFishing?.Value == true)
            {
                __instance.hit = InstantFishingConfig.HitValue?.Value ?? 100;
            }
        }
    }

    [HarmonyPatch(typeof(AIAct), nameof(AIAct.Start))]
    internal static class InstantFishingTurboPatch
    {
        [HarmonyPostfix]
        public static void Postfix(AIAct __instance)
        {
            if (__instance is AI_Fish && InstantFishingConfig.EnableTurboMode?.Value == true)
            {
                ActionMode.Adv.SetTurbo(InstantFishingConfig.TurboSpeed?.Value ?? 1);
            }
        }
    }
}