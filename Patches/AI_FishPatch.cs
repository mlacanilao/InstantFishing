using System.Collections.Generic;
using System.Linq;
using InstantFishing.Config;
using UnityEngine;

namespace InstantFishing.Patches
{
    public class AI_FishPatch
    {

        public static void OnProgressPrefix(AI_Fish.ProgressFish __instance)
        {
            if (InstantFishingConfig.EnableInstantFishing?.Value == false)
            {
                return;
            }
            
            if (__instance is null)
            {
                return;
            }
            
            if (__instance.owner?.IsPC == false)
            {
                return;
            }
            
            __instance.hit = 100;
        }
        
        public static void OnProgressCompletePostifx(AI_Fish.ProgressFish __instance)
        {
            if (__instance is null)
            {
                return;
            }
            
            if (__instance?.owner?.IsPC == false)
            {
                return;
            }

            if (InstantFishingConfig.enableInstantBonitoFlakes?.Value == true)
            {
                var inventory = EClass.pc?.things;
            
                if (inventory == null || inventory.Count == 0)
                {
                    return;
                }

                var selectedFishIds = InstantFishingConfig.SelectedFishIds;
            
                Thing caughtFish = inventory.Find(func: t => 
                    t.source.name != null &&
                    t.source._origin == "fish" &&
                    selectedFishIds.Contains(item: t.id)
                );
            
                if (caughtFish == null || caughtFish.Num <= 0)
                {
                    return;
                }
            
                int initialCount = caughtFish.Num;
            
                for (int i = 0; i < initialCount; i++)
                {
                    Thing singleFish = caughtFish.Split(a: 1);

                    AI_UseCrafter aiCrafter = new AI_UseCrafter
                    {
                        ings = new List<Thing> { singleFish },
                        crafter = new TraitSawMill(),
                    };
                    
                    Thing craftedItem = aiCrafter.crafter.Craft(ai: aiCrafter);

                    if (craftedItem != null)
                    {
                        if (InstantFishingConfig.EnableItemBlessedState?.Value == true)
                        {
                            craftedItem?.SetBlessedState(s: InstantFishingConfig.ItemBlessedState?.Value ?? BlessedState.Normal);
                        }
                        
                        if (InstantFishingConfig.EnableZeroWeight?.Value == true)
                        {
                            craftedItem?.ChangeWeight(a: 0);
                        }
                        
                        int costSP = aiCrafter.crafter.CostSP;
                        int xp = costSP * 12 *
                            (100 + 1 * 2) / 100;
                        EClass.pc.ModExp(ele: 255, a: xp);
                        EClass.pc.stamina.Mod(a: -costSP);
                        EClass._zone.AddCard(t: craftedItem, point: EClass.pc.pos);
                        EClass.pc.Pick(t: craftedItem, msg: true, tryStack: true);
                    }
                }

                if (caughtFish.Num <= 1)
                {
                    caughtFish.Destroy();
                }
            }

            if (InstantFishingConfig.enableInstantWine?.Value == true)
            {
                var inventory = EClass.pc.things;
                if (inventory == null || inventory.Count == 0)
                {
                    return;
                }
                
                var allFish = inventory.Where(t =>
                    t.source.name != null &&
                    t.source._origin == "fish" ||
                    t.source.id == "bonito"
                ).ToList();
                
                foreach (var fish in allFish)
                {
                    if (fish.Num <= 0)
                    {
                        continue;
                    }
                    
                    TraitBrewery brewery = new TraitBrewery
                    {
                        owner = __instance.owner
                    };

                    brewery.OnChildDecay(c: fish, firstDecay: true);
                }
                
                var allWine = inventory.Where(t =>
                    t.source.name != null &&
                    (t.source.name == "wine" || t.source.name == "ワイン")
                ).ToList();
                
                foreach (var wine in allWine)
                {
                    if (InstantFishingConfig.EnableItemBlessedState?.Value == true)
                    {
                        wine?.SetBlessedState(s: InstantFishingConfig.ItemBlessedState?.Value ?? BlessedState.Normal);
                    }
                    
                    if (InstantFishingConfig.EnableZeroWeight?.Value == true)
                    {
                        wine?.ChangeWeight(a: 0);
                    }
                }
            }

            if (InstantFishingConfig.EnableAutoEat?.Value == true &&
                __instance.owner?.hunger?.GetPhase() >= InstantFishingConfig.AutoEatThreshold?.Value)
            {
                __instance.owner?.InstantEat();
            }
            
            if (InstantFishingConfig.EnableAutoSleep?.Value == true &&
                __instance.owner?.sleepiness?.GetPhase() >= InstantFishingConfig.AutoSleepThreshold?.Value &&
                __instance.owner.CanSleep())
            {
                HotItemActionSleep actionSleep = new HotItemActionSleep();
                actionSleep.Perform();
            }
            
            if (InstantFishingConfig.EnableStaminaThreshold?.Value == true &&
                __instance.owner?.stamina?.value <= (InstantFishingConfig.StaminaThreshold?.Value ?? 0))
            {
                if (InstantFishingConfig.EnableAutoSleep?.Value == true &&
                    __instance.owner.CanSleep())
                {
                    HotItemActionSleep actionSleep = new HotItemActionSleep();
                    actionSleep.Perform();
                }
                else
                {
                    __instance?.owner?.Say(lang: "cancel_act_pc", c1: __instance.owner, ref1: null, ref2: null);
                    AI_Fish.shouldCancel = true;
                }
            }
        }
        
        public static bool RipplePrefix(AI_Fish.ProgressFish __instance)
        {
            if (InstantFishingConfig.EnableRippleEffect?.Value == true)
            {
                return true;
            }
            
            if (__instance?.owner?.IsPC == false)
            {
                return true;
            }
            
            return false;
        }
        
        public static void MakefishPostfix(AI_Fish __instance, Thing __result)
        {
            if (__instance?.owner?.IsPC == false)
            {
                return;
            }
            
            if (InstantFishingConfig.EnableItemMultiplier?.Value == true)
            {
                __result?.SetNum(a: __result.Num * (InstantFishingConfig.ItemMultiplier?.Value ?? 1));
            }

            if (InstantFishingConfig.EnableItemBlessedState?.Value == true)
            {
                __result?.SetBlessedState(s: InstantFishingConfig.ItemBlessedState?.Value ?? BlessedState.Normal);
            }

            if (InstantFishingConfig.EnableZeroWeight?.Value == true)
            {
                __result?.ChangeWeight(a: 0);
            }
        }
    }
}