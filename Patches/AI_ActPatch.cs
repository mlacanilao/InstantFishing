using System;
using InstantFishing.Config;

namespace InstantFishing.Patches
{
    public class AI_ActPatch
    {
        public static void StartPostfix(AIAct __instance)
        {
            if (InstantFishingConfig.EnableTurboMode?.Value == false)
            {
                return;
            }
            
            if (__instance?.owner?.IsPC == false)
            {
                return;
            }

            if (__instance is AI_Fish == false)
            {
                return;
            }
            
            ActionMode.Adv.SetTurbo(mtp: InstantFishingConfig.TurboModeSpeedMultiplier?.Value ?? 0);
        }
        
        public static void ResetPostfix(AIAct __instance)
        {
            if (InstantFishingConfig.EnableAutoDump?.Value == false)
            {
                return;
            }
            
            if (__instance is AI_Fish == false &&
                __instance is AI_Fish.ProgressFish == false &&
                __instance is TaskDump == false)
            {
                return;
            }
            
            if (__instance is TaskDump == true && __instance?.status == AIAct.Status.Success)
            {
                if (EClass.pc?.things?.IsFull() == true)
                {
                    InstantFishingConfig.EnableAutoDump.Value = !InstantFishingConfig.EnableAutoDump.Value;
                }
                
                if (InstantFishingConfig.EnableAutoFish?.Value == false)
                {
                    return;
                }
                
                FishingPointFinder.FindAndSetNearestFishingPoint();
            }

            if (EClass.pc?.things?.IsFull() == true)
            {
                EClass.pc?.SetAIImmediate(g: (AIAct) new TaskDump());
            }
        }
    }
}