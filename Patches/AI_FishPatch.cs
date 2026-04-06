using System.Collections.Generic;
using InstantFishing.Config;

namespace InstantFishing.Patches;

public class AI_FishPatch
{
    private static bool _lastPcMakefishSucceeded;
    private static Thing? _lastPcCaughtThing;

    internal static Thing? LastPcCaughtThing => _lastPcCaughtThing;

    public static void OnProgressPrefix(AI_Fish.ProgressFish __instance)
    {
        if (__instance?.owner?.IsPC == true)
        {
            _lastPcMakefishSucceeded = false;
            _lastPcCaughtThing = null;
        }

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

        var owner = __instance.owner;
        var pc = EClass.pc;

        if (owner == null || owner.IsPC == false || pc == null)
        {
            return;
        }

        if (_lastPcMakefishSucceeded == false)
        {
            return;
        }

        Thing? caughtThing = _lastPcCaughtThing;
        
        bool enableInstantReadAncientBook = InstantFishingConfig.EnableInstantReadAncientBook?.Value ?? false;
        
        if (enableInstantReadAncientBook == true)
        {
            if (IsUnreadAncientBook(thing: caughtThing))
            {
                Thing ancientBook = caughtThing!;

                if (ancientBook.trait is TraitAncientbook trait)
                {
                    bool canRead = trait.CanRead(c: pc);

                    if (ancientBook.c_charges > 0 &&
                        pc.isDead == false &&
                        canRead)
                    {
                        trait.OnRead(c: pc);
                        ancientBook.Identify(show: pc.IsPCParty, idtSource: IDTSource.Identify);
                    }
                }
            }
        }
        
        bool enableAutoEat = InstantFishingConfig.EnableAutoEat?.Value ?? false;
        if (enableAutoEat == true &&
            owner.hunger?.GetPhase() >= InstantFishingConfig.AutoEatThreshold?.Value)
        {
            owner.InstantEat();

            if (pc.isDead)
            {
                return;
            }
        }
        
        bool enableAutoSleep = InstantFishingConfig.EnableAutoSleep?.Value ?? false;
        if (enableAutoSleep == true &&
            owner.sleepiness?.GetPhase() >= (InstantFishingConfig.AutoSleepThreshold?.Value ?? 1) &&
            owner.CanSleep())
        {
            HotItemActionSleep actionSleep = new HotItemActionSleep();
            actionSleep.Perform();
            return;
        }
        
        bool enableStaminaThreshold = InstantFishingConfig.EnableStaminaThreshold?.Value ?? true;
        if (enableStaminaThreshold == true &&
            owner.stamina?.value <= (InstantFishingConfig.StaminaThreshold?.Value ?? 0))
        {
            if (enableAutoSleep == true &&
                owner.CanSleep())
            {
                HotItemActionSleep actionSleep = new HotItemActionSleep();
                actionSleep.Perform();
                return;
            }
            else
            {
                owner.Say(lang: "cancel_act_pc", c1: owner, ref1: null, ref2: null);
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
    
    public static void MakefishPostfix(Chara c, ref Thing __result)
    {
        _lastPcMakefishSucceeded = c?.IsPC == true && __result != null;
        _lastPcCaughtThing = c?.IsPC == true ? __result : null;

        if (c?.IsPC != true)
        {
            return;
        }
        
        bool enableItemMultiplier = InstantFishingConfig.EnableItemMultiplier?.Value ?? false;
        if (enableItemMultiplier == true)
        {
            __result?.SetNum(a: __result.Num * (InstantFishingConfig.ItemMultiplier?.Value ?? 1));
        }

        bool enableItemBlessedState = InstantFishingConfig.EnableItemBlessedState?.Value ?? false;
        bool enableZeroWeight = InstantFishingConfig.EnableZeroWeight?.Value ?? false;
        bool enableInstantBonitoFlakes = InstantFishingConfig.EnableInstantBonitoFlakes?.Value ?? false;
        bool enableInstantWine = InstantFishingConfig.EnableInstantWine?.Value ?? false;

        if (__result != null)
        {
            ApplyConfiguredItemModifiers(
                thing: __result,
                applyBlessedState: enableItemBlessedState,
                applyZeroWeight: enableZeroWeight,
                applyTier: true);
        }

        if (enableInstantBonitoFlakes == true &&
            IsSelectedFishForBonito(caughtThing: __result))
        {
            __result = ConvertCaughtFishToBonito(
                caughtThing: __result!,
                pc: c,
                applyBlessedState: enableItemBlessedState,
                applyZeroWeight: enableZeroWeight);
        }

        if (enableInstantWine == true &&
            IsWineConvertible(thing: __result))
        {
            __result = ConvertCaughtThingToWine(
                caughtThing: __result!,
                pc: c,
                applyBlessedState: enableItemBlessedState,
                applyZeroWeight: enableZeroWeight);
        }

        _lastPcMakefishSucceeded = __result != null;
        _lastPcCaughtThing = __result;
    }

    private static Thing ConvertCaughtFishToBonito(Thing caughtThing, Chara pc, bool applyBlessedState, bool applyZeroWeight)
    {
        Thing workingFish = caughtThing.Duplicate(num: caughtThing.Num);
        int initialCount = workingFish.Num;
        Thing? craftedStack = null;

        for (int i = 0; i < initialCount; i++)
        {
            Thing singleFish = workingFish.Split(a: 1);

            AI_UseCrafter aiCrafter = new AI_UseCrafter
            {
                ings = new List<Thing> { singleFish },
                crafter = new TraitSawMill(),
            };

            Thing craftedItem = aiCrafter.crafter.Craft(ai: aiCrafter);

            if (singleFish.isDestroyed == false)
            {
                singleFish.Destroy();
            }

            if (craftedItem == null)
            {
                continue;
            }

            ApplyConfiguredItemModifiers(
                thing: craftedItem,
                applyBlessedState: applyBlessedState,
                applyZeroWeight: applyZeroWeight,
                applyTier: false);

            if (craftedStack == null)
            {
                craftedStack = craftedItem;
            }
            else if (craftedItem.TryStackTo(to: craftedStack) == false)
            {
                craftedStack.SetNum(a: craftedStack.Num + craftedItem.Num);
                craftedItem.Destroy();
            }

            SourceRecipe.Row? recipeSource = aiCrafter.crafter.GetSource(ai: aiCrafter);
            int costSP = recipeSource?.sp ?? aiCrafter.crafter.CostSP;
            int duration = recipeSource?.time ?? 1;
            int xp = costSP * 12 * (100 + duration * 2) / 100;

            pc.ModExp(ele: 255, a: xp);
            pc.stamina.Mod(a: -costSP);
        }

        if (workingFish.isDestroyed == false)
        {
            workingFish.Destroy();
        }

        if (craftedStack == null)
        {
            return caughtThing;
        }

        if (caughtThing.isDestroyed == false)
        {
            caughtThing.Destroy();
        }

        return craftedStack;
    }

    private static Thing ConvertCaughtThingToWine(Thing caughtThing, Chara pc, bool applyBlessedState, bool applyZeroWeight)
    {
        TraitBrewery brewery = new TraitBrewery
        {
            owner = pc
        };

        string? productId = brewery.GetProductID(c: caughtThing);
        if (string.IsNullOrEmpty(value: productId))
        {
            return caughtThing;
        }

        Thing sourceThing = caughtThing.Duplicate(num: caughtThing.Num);
        Thing wine = ThingGen.Create(id: productId, idMat: -1, lv: -1).SetNum(a: sourceThing.Num);
        wine.MakeFoodRef(c1: sourceThing, c2: null);
        wine.c_priceAdd = sourceThing.GetValue(priceType: PriceType.Default, sell: false) * 125 / 100;
        brewery.OnProduce(c: wine);

        ApplyConfiguredItemModifiers(
            thing: wine,
            applyBlessedState: applyBlessedState,
            applyZeroWeight: applyZeroWeight,
            applyTier: false);

        if (caughtThing.isDestroyed == false)
        {
            caughtThing.Destroy();
        }

        return wine;
    }

    private static void ApplyConfiguredItemModifiers(Thing thing, bool applyBlessedState, bool applyZeroWeight, bool applyTier)
    {
        if (applyBlessedState == true)
        {
            thing.SetBlessedState(s: InstantFishingConfig.ItemBlessedState?.Value ?? BlessedState.Normal);
        }

        if (applyZeroWeight == true)
        {
            thing.ChangeWeight(a: 0);
        }

        if (applyTier == false)
        {
            return;
        }

        bool enableSetTier = InstantFishingConfig.EnableSetTier?.Value ?? false;
        int tierToSet = InstantFishingConfig.SetTier?.Value ?? 0;

        if (enableSetTier == true &&
            thing.source._origin == "fish" &&
            thing.id != "fish_slice" &&
            thing.tier == 0)
        {
            thing.SetTier(a: tierToSet, setTraits: true);
        }
    }

    private static bool IsSelectedFishForBonito(Thing? caughtThing)
    {
        if (caughtThing == null ||
            caughtThing.isDestroyed ||
            caughtThing.source?._origin != "fish" ||
            caughtThing.id == "fish_slice")
        {
            return false;
        }

        bool excludeTierFish = InstantFishingConfig.EnableExcludeTierFishFromBonito?.Value ?? false;
        if (excludeTierFish == true && caughtThing.tier != 0)
        {
            return false;
        }

        return InstantFishingConfig.SelectedFishIds.Contains(item: caughtThing.id);
    }

    private static bool IsWineConvertible(Thing? thing)
    {
        if (thing == null || thing.isDestroyed)
        {
            return false;
        }

        return thing.source?._origin == "fish" ||
            thing.source?.id == "bonito" ||
            thing.id == "fish_slice";
    }

    private static bool IsUnreadAncientBook(Thing? thing)
    {
        return thing != null &&
            thing.isDestroyed == false &&
            thing.trait is TraitAncientbook &&
            thing.isOn == false;
    }

}
