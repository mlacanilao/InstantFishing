using InstantFishing.Config;

namespace InstantFishing.Patches;

public class AI_ActPatch
{
    private static bool _suppressAutoDumpUntilAutoDumpConditionClears;

    public static void StartPostfix(AIAct __instance)
    {
        bool enableTurboMode = InstantFishingConfig.EnableTurboMode?.Value ?? false;
        
        if (enableTurboMode == false)
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

        int turboModeSpeedMultiplier = InstantFishing.GetEffectiveTurboModeSpeedMultiplier();
        if (AM_Adv.turbo != turboModeSpeedMultiplier)
        {
            ActionMode.Adv.SetTurbo(mtp: turboModeSpeedMultiplier);
        }
    }
    
    public static void ResetPostfix(AIAct __instance)
    {
        bool enableAutoDump = InstantFishingConfig.EnableAutoDump?.Value ?? false;
        bool enableAutoFish = InstantFishingConfig.EnableAutoFish?.Value ?? false;
        Chara? pc = EClass.pc;
        bool autoDumpConditionReached = ShouldTriggerAutoDump(pc: pc);
        
        if (enableAutoDump == false)
        {
            _suppressAutoDumpUntilAutoDumpConditionClears = false;
            return;
        }

        if (autoDumpConditionReached == false)
        {
            _suppressAutoDumpUntilAutoDumpConditionClears = false;
        }
        
        if (__instance is AI_Fish == false &&
            __instance is AI_Fish.ProgressFish == false &&
            __instance is TaskDump == false)
        {
            return;
        }
        
        if (__instance is TaskDump == true && __instance?.status == AIAct.Status.Success)
        {
            if (autoDumpConditionReached == true)
            {
                _suppressAutoDumpUntilAutoDumpConditionClears = true;
            }
            
            if (enableAutoFish == false || autoDumpConditionReached == true)
            {
                return;
            }
            
            FishingPointFinder.FindAndSetNearestFishingPoint();
        }

        if (autoDumpConditionReached == true &&
            _suppressAutoDumpUntilAutoDumpConditionClears == false)
        {
            pc?.SetAIImmediate(g: (AIAct) new TaskDump());
        }
    }

    private static bool ShouldTriggerAutoDump(Chara? pc)
    {
        if (pc?.things == null)
        {
            return false;
        }

        AutoDumpTriggerMode triggerMode = InstantFishingConfig.AutoDumpTrigger?.Value ?? Config.AutoDumpTriggerMode.FullOnly;
        int thresholdPercent = GetAutoDumpThresholdPercent();

        switch (triggerMode)
        {
            case Config.AutoDumpTriggerMode.InventoryUsagePercent:
                return GetInventoryUsagePercent(pc: pc) >= thresholdPercent;
            case Config.AutoDumpTriggerMode.CarryWeightPercent:
                return GetCarryWeightPercent(pc: pc) >= thresholdPercent;
            default:
                return pc.things.IsFull() == true;
        }
    }

    private static int GetInventoryUsagePercent(Chara pc)
    {
        int maxCapacity = pc.things.MaxCapacity;
        if (maxCapacity <= 0)
        {
            return 0;
        }

        return pc.things.Count * 100 / maxCapacity;
    }

    private static int GetCarryWeightPercent(Chara pc)
    {
        int weightLimit = pc.WeightLimit;
        if (weightLimit <= 0)
        {
            return 0;
        }

        return pc.ChildrenWeight * 100 / weightLimit;
    }

    private static int GetAutoDumpThresholdPercent()
    {
        int thresholdPercent = InstantFishingConfig.AutoDumpThreshold?.Value ?? 100;

        if (thresholdPercent < 50)
        {
            return 50;
        }

        if (thresholdPercent > 100)
        {
            return 100;
        }

        return thresholdPercent;
    }
}
