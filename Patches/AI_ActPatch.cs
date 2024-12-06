namespace InstantFishing.Patches
{
    public class AI_ActPatch
    {
        public static void Start(AIAct __instance)
        {
            if (InstantFishingConfig.EnableInstantFishingMod?.Value == true &&
                InstantFishingConfig.EnableTurboMode?.Value == true &&
                __instance is AI_Fish)
            {
                ActionMode.Adv.SetTurbo(InstantFishingConfig.TurboSpeed?.Value ?? 3);
            }
        }
    }
}