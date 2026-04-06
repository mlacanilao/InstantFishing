using InstantFishing.Config;

namespace InstantFishing.Patches;

public class CardRendererPatch
{
    public static bool PlayAnimePrefix(CardRenderer __instance, AnimeID id)
    {
        bool enableAnimations = InstantFishingConfig.EnableAnimations?.Value ?? true;
        
        if (enableAnimations == true)
        {
            return true;
        }

        if (EClass.pc?.ai is AI_Fish == false)
        {
            return true;
        }

        if (id == AnimeID.Fishing || id == AnimeID.Shiver)
        {
            return __instance?.owner?.IsPC != true;
        }

        if (id == AnimeID.Jump)
        {
            return ReferenceEquals(objA: __instance?.owner, objB: AI_FishPatch.LastPcCaughtThing) == false;
        }

        return true;
    }
}
