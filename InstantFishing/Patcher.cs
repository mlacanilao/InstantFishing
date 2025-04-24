using InstantFishing.Patches;
using HarmonyLib;
using UnityEngine;

namespace InstantFishing
{
    [HarmonyPatch]
    public class Patcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(declaringType: typeof(AIAct), methodName: nameof(AIAct.Start))]
        public static void AIActStart(AIAct __instance)
        {
            AI_ActPatch.StartPostfix(__instance: __instance);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(declaringType: typeof(AIAct), methodName: nameof(AIAct.Reset))]
        public static void AIActReset(AIAct __instance)
        {
            AI_ActPatch.ResetPostfix(__instance: __instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(AI_Fish.ProgressFish), methodName: nameof(AI_Fish.ProgressFish.OnProgress))]
        public static void AI_FishOnProgress(AI_Fish.ProgressFish __instance)
        {
            AI_FishPatch.OnProgressPrefix(__instance: __instance);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(declaringType: typeof(AI_Fish.ProgressFish), methodName: nameof(AI_Fish.ProgressFish.OnProgressComplete))]
        public static void AI_FishProgressFishOnProgressComplete(AI_Fish.ProgressFish __instance)
        {
            AI_FishPatch.OnProgressCompletePostfix(__instance: __instance);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(AI_Fish.ProgressFish), methodName: nameof(AI_Fish.ProgressFish.Ripple))]
        public static bool AI_FishProgressFishRipple(AI_Fish.ProgressFish __instance)
        {
            return AI_FishPatch.RipplePrefix(__instance: __instance);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(declaringType: typeof(AI_Fish), methodName: nameof(AI_Fish.Makefish))]
        public static void AI_FishMakefish(AI_Fish __instance, ref Thing __result)
        {
            AI_FishPatch.MakefishPostfix(__instance: __instance, __result: __result);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(ElementContainer), methodName: nameof(ElementContainer.ModExp))]
        public static void ElementContainerModExp(ElementContainer __instance, int ele, ref int a)
        {
            ElementContainerPatch.ModExpPrefix(__instance: __instance, ele: ele, a: ref a);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(CardRenderer), methodName: nameof(CardRenderer.PlayAnime), argumentTypes: new[] { typeof(AnimeID), typeof(Vector3), typeof(bool) })]
        public static bool CardRendererPlayAnime(CardRenderer __instance, AnimeID id)
        {
            return CardRendererPatch.PlayAnimePrefix(__instance: __instance, id: id);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(Card), methodName: nameof(Card.PlaySound))]
        public static bool CardPlaySound(Card __instance, string id)
        {
            return CardPatch.PlaySoundPrefix(__instance: __instance, id: id);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(Card), methodName: nameof(Card.SetTier))]
        public static bool CardSetTier(Card __instance, int a)
        {
            return CardPatch.SetTierPrefix(__instance: __instance);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(Cell), methodName: nameof(Cell.IsTopWaterAndNoSnow), methodType: MethodType.Getter)]
        public static bool CellIsTopWaterAndNoSnow(Cell __instance, ref bool __result)
        {
            return CellPatch.IsTopWaterAndNoSnowPrefix(__instance: __instance, __result: ref __result);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Advance))]
        public static void LayerSleepAdvance(LayerSleep __instance)
        {
            LayerSleepPatch.AdvancePostfix(__instance: __instance);
        }
    }
}