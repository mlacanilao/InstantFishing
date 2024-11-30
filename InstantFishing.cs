using BepInEx;
using HarmonyLib;

namespace InstantFishing
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.instantfishing";
        internal const string Name = "Instant Fishing";
        internal const string Version = "1.2.0.0";
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
            if (InstantFishingConfig.EnableInstantFishing?.Value == true &&
                InstantFishingConfig.EnableTurboMode?.Value == true &&
                __instance is AI_Fish)
            {
                ActionMode.Adv.SetTurbo(InstantFishingConfig.TurboSpeed?.Value ?? 3);
            }
        }
    }

    [HarmonyPatch(typeof(AI_Fish.ProgressFish), nameof(AI_Fish.ProgressFish.OnProgressComplete))]
    internal static class InstantFishingStaminaPatch
    {
        [HarmonyPostfix]
        public static void Postfix(AI_Fish.ProgressFish __instance)
        {
            if (InstantFishingConfig.EnableInstantFishing?.Value == true &&
                InstantFishingConfig.EnableStaminaCheck?.Value == true &&
                __instance.owner.stamina.value <= (InstantFishingConfig.StaminaThreshold?.Value ?? 1))
            {
                ELayer.pc.TalkRaw(text: $"You are too tired to continue fishing.", ref1: null, ref2: null, forceSync: false);
                AI_Fish.shouldCancel = InstantFishingConfig.ShouldCancel?.Value ?? true;
            }
        }
    }
}
