using HarmonyLib;
using InstantFishing.Config;

namespace InstantFishing.Patches;

public class LayerSleepPatch
{
    public struct SleepAdvanceState
    {
        public bool ShouldResumeFishing;
        public int Min;
        public int MaxMin;
    }

    private static readonly AccessTools.FieldRef<LayerSleep, int> MinField =
        AccessTools.FieldRefAccess<LayerSleep, int>(fieldName: "min");

    private static readonly AccessTools.FieldRef<LayerSleep, int> MaxMinField =
        AccessTools.FieldRefAccess<LayerSleep, int>(fieldName: "maxMin");

    public static void AdvancePrefix(LayerSleep __instance, ref SleepAdvanceState __state)
    {
        bool enableAutoFish = InstantFishingConfig.EnableAutoFish?.Value ?? false;

        if (enableAutoFish == false || __instance == null)
        {
            __state = default;
            return;
        }

        int min = MinField(instance: __instance);
        int maxMin = MaxMinField(instance: __instance);

        __state = new SleepAdvanceState
        {
            ShouldResumeFishing = min > maxMin,
            Min = min,
            MaxMin = maxMin
        };

    }

    public static void AdvancePostfix(SleepAdvanceState __state)
    {
        if (__state.ShouldResumeFishing == false)
        {
            return;
        }

        FishingPointFinder.FindAndSetNearestFishingPoint();
    }
}
