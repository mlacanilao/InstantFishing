using InstantFishing.Config;

namespace InstantFishing.Patches
{
    public class CardPatch
    {
        public static bool PlaySoundPrefix(Card __instance, string id)
        {
            if (InstantFishingConfig.EnableSounds?.Value == true)
            {
                return true;
            }
            
            if (__instance?.Chara?.IsPC == false)
            {
                return true;
            }
            
            if (EClass.pc?.ai is AI_Fish == false)
            {
                return true;
            }

            if (id != "fish_fight" && id != "fish_get" && id != "fish_cast" && id != "fish_splash")
            {
                return true;
            }
            
            return false;
        }
    }
}