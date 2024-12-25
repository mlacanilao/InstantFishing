using System.Collections.Generic;
using System.Linq;
using InstantFishing.Config;
using UnityEngine;

namespace InstantFishing.Patches
{
    public class AI_FishPatch
    {

        public static void OnProgressPrefix(AI_Fish.ProgressFish __instance)
        {
            if (InstantFishingConfig.EnableInstantFishing?.Value == false)
            {
                return;
            }
            
            if (__instance is null)
            {
                return;
            }
            
            if (__instance.owner?.IsPC == false)
            {
                return;
            }
            
            __instance.hit = 100;
        }
        
        public static void OnProgressCompletePostifx(AI_Fish.ProgressFish __instance)
        {
            if (__instance is null)
            {
                return;
            }
            
            if (__instance?.owner?.IsPC == false)
            {
                return;
            }

            if (InstantFishingConfig.EnableAutoEat?.Value == true &&
                __instance.owner?.hunger?.GetPhase() >= InstantFishingConfig.AutoEatThreshold?.Value)
            {
                __instance.owner?.InstantEat();
            }
            
            if (InstantFishingConfig.EnableAutoSleep?.Value == true &&
                __instance.owner?.sleepiness?.GetPhase() >= InstantFishingConfig.AutoSleepThreshold?.Value &&
                __instance.owner.CanSleep())
            {
                HotItemActionSleep actionSleep = new HotItemActionSleep();
                actionSleep.Perform();
            }

            if (InstantFishingConfig.EnableStaminaThreshold?.Value == true &&
                __instance.owner?.stamina?.value <= (InstantFishingConfig.StaminaThreshold?.Value ?? 0))
            {
                if (InstantFishingConfig.EnableAutoSleep?.Value == true &&
                    __instance.owner.CanSleep())
                {
                    HotItemActionSleep actionSleep = new HotItemActionSleep();
                    actionSleep.Perform();
                }
                else
                {
                    __instance?.owner?.Say(lang: "cancel_act_pc", c1: __instance.owner, ref1: null, ref2: null);
                    AI_Fish.shouldCancel = true;
                }
            }
        }
        
        public static bool RipplePrefix(AI_Fish.ProgressFish __instance)
        {
            if (InstantFishingConfig.EnableRippleEffect?.Value == true)
            {
                return true;
            }
            
            if (__instance?.owner?.IsPC == false)
            {
                return true;
            }
            
            return false;
        }
        
        public static void MakefishPostfix(AI_Fish __instance, Thing __result)
        {
            if (__instance?.owner?.IsPC == false)
            {
                return;
            }
            
            if (InstantFishingConfig.EnableItemMultiplier?.Value == true)
            {
                __result?.SetNum(a: __result.Num * (InstantFishingConfig.ItemMultiplier?.Value ?? 1));
            }

            if (InstantFishingConfig.EnableItemBlessedState?.Value == true)
            {
                __result?.SetBlessedState(s: InstantFishingConfig.ItemBlessedState?.Value ?? BlessedState.Normal);
            }

            if (InstantFishingConfig.EnableZeroWeight?.Value == true)
            {
                __result?.ChangeWeight(a: 0);
            }
        }
    }
}