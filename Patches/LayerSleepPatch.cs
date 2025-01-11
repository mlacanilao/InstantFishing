using InstantFishing.Config;

namespace InstantFishing.Patches
{
    public class LayerSleepPatch
    {
        public static void AdvancePostfix(LayerSleep __instance)
        {
            bool enableAutoFish = InstantFishingConfig.EnableAutoFish?.Value ?? false;
            
            if (enableAutoFish == false)
            {
                return;
            }
            
            FishingPointFinder.FindAndSetNearestFishingPoint();
        }
    }
}