using InstantFishing.Config;

namespace InstantFishing.Patches
{
    public class CardPatch
    {
        public static bool PlaySoundPrefix(Card __instance, string id)
        {
            bool enableSounds = InstantFishingConfig.EnableSounds?.Value ?? true;
            
            if (enableSounds == true)
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
        
        public static bool SetTierPrefix(Card __instance)
        {
            bool enableSetTier = InstantFishingConfig.EnableSetTier?.Value ?? false;

            if (enableSetTier == false)
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
            
            if (__instance?.sourceCard?._origin == "fish" &&
                __instance?.id != "fish_slice" &&
                __instance?.tier != 0)
            {
                return false;
            }
            
            return true;
        }

        public static bool ModNumPrefix(Card __instance, int a)
        {
            bool enableNoBaitConsumption = InstantFishingConfig.EnableNoBaitConsumption?.Value ?? false;

            if (enableNoBaitConsumption == false)
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

            if (__instance?.trait is TraitBait == false)
            {
                return true;
            }

            if (a < 0)
            {
                return false;
            }

            return true;
        }
    }
}