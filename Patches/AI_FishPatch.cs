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
        
        public static void OnProgressCompletePostfix(AI_Fish.ProgressFish __instance)
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
                
                Thing caughtFish = null;
                
                CheckForFishInThings(inventory, selectedFishIds, ref caughtFish);
            
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
                var inventory = EClass.pc?.things;
                if (inventory == null || inventory.Count == 0)
                {
                    return;
                }
                
                Thing fishOrBonito = null;
                Card container = null;

                FindFirstFishOrBonito(inventory, ref fishOrBonito, ref container);
                
                if (fishOrBonito != null && fishOrBonito.Num > 0)
                {
                    var ownerCard = container ?? EClass.pc;
                    
                    TraitBrewery brewery = new TraitBrewery
                    {
                        owner = ownerCard
                    };

                    brewery.OnChildDecay(c: fishOrBonito, firstDecay: true);
                }
                
                List<Thing> allWine = new List<Thing>();
                CollectWineItems(inventory, allWine);
                
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
            
            bool enableInstantReadAncientBook = InstantFishingConfig.EnableInstantReadAncientBook?.Value ?? false;
            
            if (enableInstantReadAncientBook == true)
            {
                var inventory = EClass.pc?.things;
                if (inventory == null || inventory.Count == 0)
                {
                    return;
                }
                
                Thing ancientBook = null;
                Thing container = null;

                FindFirstAncientBook(inventory, ref ancientBook, ref container);
                
                if (ancientBook != null && ancientBook?.trait is TraitAncientbook trait && ancientBook.c_charges > 0)
                {
                    for (int i = 0; i < ancientBook.c_charges; i++)
                    {
                        if (EClass.pc.isDead)
                            break;
                        
                        trait.OnRead(EClass.pc);
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

        private static void CheckForFishInThings(List<Thing> things, List<string> selectedFishIds, ref Thing caughtFish)
        {
            if (things == null || things.Count == 0 || caughtFish != null)
                return;

            foreach (var thing in things)
            {
                if (thing.source?.name != null &&
                    thing.source._origin == "fish" &&
                    thing.id != "fish_slice" &&
                    selectedFishIds.Contains(thing.id))
                {
                    caughtFish = thing;
                    return;
                }

                if (thing.things != null && thing.things.Count > 0)
                {
                    CheckForFishInThings(thing.things, selectedFishIds, ref caughtFish);

                    if (caughtFish != null)
                        return;
                }
            }
        }
        
        private static void FindFirstFishOrBonito(List<Thing> things, ref Thing result, ref Card container, Thing parent = null)
        {
            if (things == null || things.Count == 0 || result != null)
                return;

            foreach (var thing in things)
            {
                if (
                    ((thing.source?.name != null && thing.source._origin == "fish") || 
                     thing.source?.id == "bonito") &&
                    thing.id != "fish_slice"
                )
                {
                    result = thing;
                    container = parent;
                    return;
                }

                if (thing.things != null && thing.things.Count > 0)
                {
                    FindFirstFishOrBonito(thing.things, ref result, ref container, parent: thing);

                    if (result != null)
                        return;
                }
            }
        }

        
        private static void CollectWineItems(List<Thing> things, List<Thing> matches)
        {
            if (things == null || things.Count == 0)
                return;

            foreach (var thing in things)
            {
                if (thing.source?.name == "wine" || thing.source?.name == "ワイン")
                {
                    matches.Add(thing);
                }

                if (thing.things != null && thing.things.Count > 0)
                {
                    CollectWineItems(thing.things, matches);
                }
            }
        }
        
        private static void FindFirstAncientBook(List<Thing> things, ref Thing result, ref Thing container, Thing parent = null)
        {
            if (things == null || things.Count == 0 || result != null)
                return;

            foreach (var thing in things)
            {
                if (thing.trait is TraitAncientbook && thing.isOn == false)
                {
                    result = thing;
                    container = parent;
                    return;
                }

                if (thing.things != null && thing.things.Count > 0)
                {
                    FindFirstAncientBook(thing.things, ref result, ref container, parent: thing);

                    if (result != null)
                        return;
                }
            }
        }

    }
}