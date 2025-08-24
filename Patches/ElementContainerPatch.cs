using InstantFishing.Config;

namespace InstantFishing.Patches
{
    public class ElementContainerPatch
    {
        public static void ModExpPrefix(ElementContainer __instance, int ele, ref float a)
        {
            bool enableExperienceMultiplier = InstantFishingConfig.EnableExperienceMultiplier?.Value ?? false;
            
            if (enableExperienceMultiplier == false)
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