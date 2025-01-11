using InstantFishing.Config;

namespace InstantFishing.Patches
{
    public class CardRendererPatch
    {
        public static bool PlayAnimePrefix(CardRenderer __instance, AnimeID id)
        {
            bool enableAnimations = InstantFishingConfig.EnableAnimations?.Value ?? true;
            
            if (enableAnimations == true)
            {
                return true;
            }
            
            if (__instance?.owner?.IsPC == false)
            {
                return true;
            }
            
            if (EClass.pc?.ai is AI_Fish == false)
            {
                return true;
            }

            if (id != AnimeID.Fishing && id != AnimeID.Shiver && id != AnimeID.Jump)
            {
                return true;
            }
            
            return false;
        }
    }
}