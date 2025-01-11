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
            bool enableInstantFishing = InstantFishingConfig.EnableInstantFishing?.Value ?? false;
            
            if (enableInstantFishing == false)
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

            bool enableInstantBonitoFlakes = InstantFishingConfig.EnableInstantBonitoFlakes?.Value ?? false;
            bool enableItemBlessedState = InstantFishingConfig.EnableItemBlessedState?.Value ?? false;
            bool enableZeroWeight = InstantFishingConfig.EnableZeroWeight?.Value ?? false;

            if (enableInstantBonitoFlakes == true)
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
                        if (enableItemBlessedState == true)
                        {
                            craftedItem?.SetBlessedState(s: InstantFishingConfig.ItemBlessedState?.Value ?? BlessedState.Normal);
                        }
                        
                        if (enableZeroWeight == true)
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
            
            bool enableInstantWine = InstantFishingConfig.EnableInstantWine?.Value ?? false;

            if (enableInstantWine == true)
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
                    if (enableItemBlessedState == true)
                    {
                        wine?.SetBlessedState(s: InstantFishingConfig.ItemBlessedState?.Value ?? BlessedState.Normal);
                    }
                    
                    if (enableZeroWeight == true)
                    {
                        wine?.ChangeWeight(a: 0);
                    }
                }
            }
            
            bool enableAutoEat = InstantFishingConfig.EnableAutoEat?.Value ?? false;
            if (enableAutoEat == true &&
                __instance.owner?.hunger?.GetPhase() >= InstantFishingConfig.AutoEatThreshold?.Value)
            {
                __instance.owner?.InstantEat();
            }
            
            bool enableAutoSleep = InstantFishingConfig.EnableAutoSleep?.Value ?? false;
            if (enableAutoSleep == true &&
                __instance.owner?.sleepiness?.GetPhase() >= (InstantFishingConfig.AutoSleepThreshold?.Value ?? 1) &&
                __instance.owner.CanSleep())
            {
                HotItemActionSleep actionSleep = new HotItemActionSleep();
                actionSleep.Perform();
            }
            
            bool enableStaminaThreshold = InstantFishingConfig.EnableStaminaThreshold?.Value ?? true;
            if (enableStaminaThreshold == true &&
                __instance.owner?.stamina?.value <= (InstantFishingConfig.StaminaThreshold?.Value ?? 0))
            {
                if (enableAutoSleep == true &&
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
            bool enableRippleEffect = InstantFishingConfig.EnableRippleEffect?.Value ?? true;
            
            if (enableRippleEffect == true)
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
            
            bool enableItemMultiplier = InstantFishingConfig.EnableItemMultiplier?.Value ?? false;
            if (enableItemMultiplier == true)
            {
                __result?.SetNum(a: __result.Num * (InstantFishingConfig.ItemMultiplier?.Value ?? 1));
            }

            bool enableItemBlessedState = InstantFishingConfig.EnableItemBlessedState?.Value ?? false;
            if (enableItemBlessedState == true)
            {
                __result?.SetBlessedState(s: InstantFishingConfig.ItemBlessedState?.Value ?? BlessedState.Normal);
            }

            bool enableZeroWeight = InstantFishingConfig.EnableZeroWeight?.Value ?? false;
            if (enableZeroWeight == true)
            {
                __result?.ChangeWeight(a: 0);
            }
        }
    }
}