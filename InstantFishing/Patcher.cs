using InstantFishing.Patches;
using HarmonyLib;

namespace InstantFishing
{
    [HarmonyPatch]
    public class Patcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AIAct), nameof(AIAct.Start))]
        public static void Start(AIAct __instance)
        {
            AI_ActPatch.Start(__instance: __instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AI_Fish.ProgressFish), nameof(AI_Fish.ProgressFish.OnProgress))]
        public static void OnProgress(AI_Fish.ProgressFish __instance)
        {
            AI_FishPatch.OnProgress(__instance: __instance);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AI_Fish.ProgressFish), nameof(AI_Fish.ProgressFish.OnProgressComplete))]
        public static void OnProgressComplete(AI_Fish.ProgressFish __instance)
        {
            AI_FishPatch.OnProgressComplete(__instance: __instance);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(AI_Fish.ProgressFish), nameof(AI_Fish.ProgressFish.Ripple))]
        public static bool Ripple()
        {
            return AI_FishPatch.Ripple();
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(declaringType: typeof(UIContextMenuManager), methodName: nameof(UIContextMenuManager.Create))]
        public static void UIContextMenuManager_Create(UIContextMenuManager __instance, string menuName = "ContextMenu", bool destroyOnHide = true)
        {
            UIContextMenuManagerPatch.Create(__instance: __instance, menuName: menuName, destroyOnHide: destroyOnHide);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AI_Fish), nameof(AI_Fish.Makefish))]
        public static void Makefish(ref Thing __result)
        {
            AI_FishPatch.Makefish(__result: __result);
        }
    }
}