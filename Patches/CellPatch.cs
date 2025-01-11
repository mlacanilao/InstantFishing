using InstantFishing.Config;

namespace InstantFishing.Patches
{
    public class CellPatch
    {
        public static bool IsTopWaterAndNoSnowPrefix(Cell __instance, ref bool __result)
        {
            bool enableWinterFishing = InstantFishingConfig.EnableWinterFishing?.Value ?? false;
            
            if (enableWinterFishing == false)
            {
                return true;
            }
            __result = __instance.IsTopWater;
            return false;
        }
    }
}