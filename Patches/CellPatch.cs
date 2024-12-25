using InstantFishing.Config;

namespace InstantFishing.Patches
{
    public class CellPatch
    {
        public static bool IsTopWaterAndNoSnowPrefix(Cell __instance, ref bool __result)
        {
            if (InstantFishingConfig.EnableWinterFishing?.Value == false)
            {
                return true;
            }
            __result = __instance.IsTopWater;
            return false;
        }
    }
}