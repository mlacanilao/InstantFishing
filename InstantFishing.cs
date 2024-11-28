using BepInEx;
using HarmonyLib;

namespace InstantFishing
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.instantfishing";
        internal const string Name = "Instant Fishing";
        internal const string Version = "1.0.0.0";
    }

    [BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
    internal partial class InstantFishing : BaseUnityPlugin
    {
        internal static InstantFishing Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            InstantFishingConfig.LoadConfig(config: Config);
            var harmony = new Harmony(id: ModInfo.Guid);
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(declaringType: typeof(AI_Fish.ProgressFish))]
    [HarmonyPatch(methodName: "OnProgress")]
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
    
    [HarmonyPatch(typeof(AIAct))]
    [HarmonyPatch(nameof(AIAct.Start))]
    internal static class InstantFishingTurboPatch
    {
        [HarmonyPostfix]
        public static void Postfix(AIAct __instance)
        {
            if (__instance is AI_Fish aiFishInstance && InstantFishingConfig.EnableTurboMode?.Value == true)
            {
                ActionMode.Adv.SetTurbo(mtp: InstantFishingConfig.TurboSpeed?.Value ?? 1);
            }
        }
    }
}