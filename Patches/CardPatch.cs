using InstantFishing.Config;

namespace InstantFishing.Patches;

public class CardPatch
{
    public static bool PlaySoundPrefix(Card __instance, string id)
    {
        if (ShouldSuppressFishingSounds() == false)
        {
            return true;
        }

        if (__instance?.Chara?.IsPC == false)
        {
            return true;
        }

        if (id != "fish_fight" && id != "fish_get" && id != "fish_cast")
        {
            return true;
        }
        
        return false;
    }

    public static bool PointPlaySoundPrefix(string id)
    {
        if (ShouldSuppressFishingSounds() == false)
        {
            return true;
        }

        if (id != "fish_splash")
        {
            return true;
        }

        return false;
    }

    public static bool ModNumPrefix(Card __instance, int a)
    {
        bool enableNoBaitConsumption = InstantFishingConfig.EnableNoBaitConsumption?.Value ?? false;

        if (enableNoBaitConsumption == false)
        {
            return true;
        }
        
        if (EClass.pc?.ai is AI_Fish == false)
        {
            return true;
        }

        if (a >= 0)
        {
            return true;
        }

        if (ReferenceEquals(objA: EClass.player?.eqBait, objB: __instance) == false)
        {
            return true;
        }

        return false;
    }

    private static bool ShouldSuppressFishingSounds()
    {
        bool enableSounds = InstantFishingConfig.EnableSounds?.Value ?? true;
        return enableSounds == false && EClass.pc?.ai is AI_Fish;
    }
}
