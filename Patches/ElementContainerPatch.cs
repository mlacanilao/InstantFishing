using InstantFishing.Config;

namespace InstantFishing.Patches
{
    public class ElementContainerPatch
    {
        public static void ModExpPrefix(ElementContainer __instance, int ele, ref int a)
        {
            if (InstantFishingConfig.EnableExperienceMultiplier?.Value == false)
            {
                return;
            }
            
            if (__instance?.Chara?.IsPC == false)
            {
                return;
            }
            
            if (EClass.pc?.ai is AI_Fish == false)
            {
                return;
            }
            
            if (ele != 245)
            {
                return;
            }
            
            a *= InstantFishingConfig.ExperienceMultiplier?.Value ?? 1;
        }
    }
}