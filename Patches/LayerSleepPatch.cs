using InstantFishing.Config;

namespace InstantFishing.Patches
{
    public class LayerSleepPatch
    {
        public static void AdvancePostfix(LayerSleep __instance)
        {
            if (InstantFishingConfig.EnableAutoFish?.Value == false)
            {
                return;
            }
            
            FishingPointFinder.FindAndSetNearestFishingPoint();
        }
    }
}