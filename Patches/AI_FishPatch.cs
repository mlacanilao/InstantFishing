using UnityEngine;

namespace InstantFishing.Patches
{
    public class AI_FishPatch
    {
        public static void OnProgress(AI_Fish.ProgressFish __instance)
        {
            if (InstantFishingConfig.EnableInstantFishingMod?.Value == true &&
                InstantFishingConfig.EnableHitValue?.Value == true)
            {
                __instance.hit = InstantFishingConfig.HitValue?.Value ?? 100;
            }
        }
        
        public static void OnProgressComplete(AI_Fish.ProgressFish __instance)
        {
            if (InstantFishingConfig.EnableInstantFishingMod?.Value == true &&
                InstantFishingConfig.EnableStaminaCheck?.Value == true &&
                __instance.owner.stamina.value <= (InstantFishingConfig.StaminaThreshold?.Value ?? 1))
            {
                ELayer.pc.TalkRaw(text: $"You are too tired to continue fishing.", ref1: null, ref2: null, forceSync: false);
                AI_Fish.shouldCancel = InstantFishingConfig.ShouldCancel?.Value ?? true;
            }
        }
        
        public static bool Ripple()
        {
            if (InstantFishingConfig.EnableInstantFishingMod?.Value == true &&
                InstantFishingConfig.EnableRipple?.Value == false)
            {
                return false;
            }
            return true;
        }
        
        public static void Makefish(Thing __result)
        {
            if (InstantFishingConfig.EnableInstantFishingMod?.Value == true &&
                InstantFishingConfig.EnableItemMultiplier?.Value == true)
            {
                __result?.SetNum(a: __result.Num * (InstantFishingConfig.ItemMultiplier?.Value ?? 1));
            }

            if (InstantFishingConfig.EnableInstantFishingMod?.Value == true &&
                InstantFishingConfig.EnableItemBlessedState?.Value == true)
            {
                BlessedState newState = InstantFishingConfig.IsBlessed?.Value == true
                    ? BlessedState.Blessed
                    : BlessedState.Normal;

                __result?.SetBlessedState(s: newState);
            }
        }
    }
}