using InstantFishing.Config;

namespace InstantFishing.Patches
{
    public class StatsStaminaPatch
    {
        public static bool ModPrefix()
        {
            bool enableNoStaminaCost = InstantFishingConfig.EnableNoStaminaCost?.Value ?? false;

            if (enableNoStaminaCost == false)
            {
                return true;
            }

            if (EClass.core.IsGameStarted == false ||
                BaseStats.CC?.IsPC == false)
            {
                return true;
            }
            
            var currentAI = BaseStats.CC?.ai;

            if (currentAI is AI_Fish == false)
            {
                return true;
            }
            
            return false;
        }
    }
}