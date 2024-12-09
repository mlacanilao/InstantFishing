using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InstantFishing.Patches
{
    public class AI_FishPatch
    {
        public static void OnProgress(AI_Fish.ProgressFish __instance)
        {
            if (InstantFishingConfig.EnableInstantFishingMod?.Value == true &&
                InstantFishingConfig.EnableHitValue?.Value == true)
            {
                __instance.hit = InstantFishingConfig.HitValue?.Value ?? 100;
            }
        }
        
        public static void OnProgressComplete(AI_Fish.ProgressFish __instance)
        {
            if (InstantFishingConfig.EnableInstantFishingMod?.Value == true &&
                InstantFishingConfig.EnableAutoBonitoFlakes?.Value == true)
            {
                if (__instance.owner != EClass.pc) return;
                
                var inventory = EClass.pc.things;
                if (inventory == null || inventory.Count == 0)
                {
                    return;
                }
                
                var excludedFishSet = InstantFishingConfig.GetExcludedBonitoFlakesFishSet();

                Thing caughtFish = inventory.Find(func: t => 
                        t.source.name != null &&
                        t.source._origin == "fish" &&
                        !excludedFishSet.Contains(item: t.source.name)
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

            if (InstantFishingConfig.EnableInstantFishingMod?.Value == true &&
                InstantFishingConfig.EnableAutoWine?.Value == true)
            {
                if (__instance.owner != EClass.pc)
                {
                    return;
                }

                var inventory = EClass.pc.things;
                if (inventory == null || inventory.Count == 0)
                {
                    return;
                }

                var allFish = inventory.Where(t =>
                    t.source.name != null &&
                    t.source._origin == "fish" ||
                    (t.source.name == "bonito flakes" || t.source.name == "かつおぶし")
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
            }
            
            if (InstantFishingConfig.EnableInstantFishingMod?.Value == true &&
                InstantFishingConfig.EnableStaminaCheck?.Value == true &&
                __instance.owner.stamina.value <= (InstantFishingConfig.StaminaThreshold?.Value ?? 1))
            {
                ELayer.pc.TalkRaw(text: $"You are too tired.", ref1: null, ref2: null, forceSync: false);
                AI_Fish.shouldCancel = InstantFishingConfig.ShouldCancel?.Value ?? true;
            }
        }
        
        public static bool Ripple()
        {
            if (InstantFishingConfig.EnableInstantFishingMod?.Value == true &&
                InstantFishingConfig.EnableRipple?.Value == false)
            {
                return false;
            }
            return true;
        }
        
        public static void Makefish(Thing __result)
        {
            if (InstantFishingConfig.EnableInstantFishingMod?.Value == true &&
                InstantFishingConfig.EnableItemMultiplier?.Value == true)
            {
                __result?.SetNum(a: __result.Num * (InstantFishingConfig.ItemMultiplier?.Value ?? 1));
            }

            if (InstantFishingConfig.EnableInstantFishingMod?.Value == true &&
                InstantFishingConfig.EnableItemBlessedState?.Value == true)
            {
                BlessedState newState = InstantFishingConfig.IsBlessed?.Value == true
                    ? BlessedState.Blessed
                    : BlessedState.Normal;

                __result?.SetBlessedState(s: newState);
            }

            if (InstantFishingConfig.EnableInstantFishingMod?.Value == true &&
                InstantFishingConfig.EnableZeroWeight?.Value == true)
            {
                __result?.ChangeWeight(a: 0);
            }
        }
    }
}