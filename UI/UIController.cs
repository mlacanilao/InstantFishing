using System.Collections.Generic;
using EvilMask.Elin.ModOptions;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using EvilMask.Elin.ModOptions.UI;
using InstantFishing.Config;
using UnityEngine;

namespace InstantFishing.UI;

public static class UIController
{
    private const int FishGridColumns = 4;
    
    public static void RegisterUI()
    {
        foreach (var obj in ModManager.ListPluginObject)
        {
            if (obj is BaseUnityPlugin plugin && plugin.Info.Metadata.GUID == ModInfo.ModOptionsGuid)
            {
                var controller = ModOptionController.Register(guid: ModInfo.Guid, tooptipId: "mod.tooltip");
                if (controller == null)
                {
                    InstantFishing.LogError(message: "Failed to register Mod Options controller.");
                    return;
                }

                SetTranslations(controller: controller);
                
                var assemblyLocation = Path.GetDirectoryName(path: Assembly.GetExecutingAssembly().Location) ?? string.Empty;
                var xmlPath = Path.Combine(path1: assemblyLocation, path2: "InstantFishingConfig.xml");
                InstantFishingConfig.InitializeXmlPath(xmlPath: xmlPath);
        
                var xlsxPath = Path.Combine(path1: assemblyLocation, path2: "translations.xlsx");
                InstantFishingConfig.InitializeTranslationXlsxPath(xlsxPath: xlsxPath);
                
                if (File.Exists(path: InstantFishingConfig.XmlPath))
                {
                    controller.SetPreBuildWithXml(xml: File.ReadAllText(path: InstantFishingConfig.XmlPath));
                }
                
                if (File.Exists(path: InstantFishingConfig.TranslationXlsxPath))
                {
                    controller.SetTranslationsFromXslx(path: InstantFishingConfig.TranslationXlsxPath);
                }
                
                RegisterEvents(controller: controller);
            }
        }
    }
    
    private static void RegisterEvents(ModOptionController controller)
    {
        controller.OnBuildUI += builder =>
        {
            for (int i = 1; i <= 13; i++)
            {
                string hlayoutId = $"hlayout{i:D2}";

                var hlayout = builder.GetPreBuild<OptHLayout>(id: hlayoutId);
    
                if (hlayout != null)
                {
                    hlayout.Base.childForceExpandHeight = false;
                }
            }
            
            for (int i = 1; i <= 57; i++)
            {
                string vlayoutId = $"vlayout{i:D2}";

                var vlayout = builder.GetPreBuild<OptVLayout>(id: vlayoutId);
    
                if (vlayout != null)
                {
                    vlayout.Base.childForceExpandHeight = false;
                }
            }
            
            // Cheats
            var enableInstantFishingToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableInstantFishingToggle");
            if (enableInstantFishingToggle == null)
            {
                return;
            }
            enableInstantFishingToggle.Checked = InstantFishingConfig.EnableInstantFishing.Value;
            enableInstantFishingToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableInstantFishing.Value = isChecked;
            };
            
            var enableInstantReadAncientBookToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableInstantReadAncientBookToggle");
            if (enableInstantReadAncientBookToggle == null)
            {
                return;
            }
            enableInstantReadAncientBookToggle.Checked = InstantFishingConfig.EnableInstantReadAncientBook.Value;
            enableInstantReadAncientBookToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableInstantReadAncientBook.Value = isChecked;
            };
            
            var enableNoStaminaCostToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableNoStaminaCostToggle");
            if (enableNoStaminaCostToggle == null)
            {
                return;
            }
            enableNoStaminaCostToggle.Checked = InstantFishingConfig.EnableNoStaminaCost.Value;
            enableNoStaminaCostToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableNoStaminaCost.Value = isChecked;
            };
            
            var enableNoBaitConsumptionToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableNoBaitConsumptionToggle");
            if (enableNoBaitConsumptionToggle == null)
            {
                return;
            }
            enableNoBaitConsumptionToggle.Checked = InstantFishingConfig.EnableNoBaitConsumption.Value;
            enableNoBaitConsumptionToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableNoBaitConsumption.Value = isChecked;
            };
            
            var enableExperienceMultiplierToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableExperienceMultiplierToggle");
            if (enableExperienceMultiplierToggle == null)
            {
                return;
            }
            enableExperienceMultiplierToggle.Checked = InstantFishingConfig.EnableExperienceMultiplier.Value;
            enableExperienceMultiplierToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableExperienceMultiplier.Value = isChecked;
            };
            
            var experienceSlider = GetRequiredPreBuild<OptSlider>(builder: builder, id: "experienceSlider");
            if (experienceSlider == null)
            {
                return;
            }
            experienceSlider.Title = InstantFishingConfig.ExperienceMultiplier.Value.ToString();
            experienceSlider.Value = InstantFishingConfig.ExperienceMultiplier.Value;
            experienceSlider.Step = 1;
            experienceSlider.OnValueChanged += value =>
            {
                experienceSlider.Title = value.ToString();
                InstantFishingConfig.ExperienceMultiplier.Value = (int)value;
            };
            
            var experienceSliderDropdown = GetRequiredPreBuild<OptDropdown>(builder: builder, id: "experienceSliderDropdown");
            if (experienceSliderDropdown == null)
            {
                return;
            }
            experienceSliderDropdown.OnValueChanged += index =>
            {
                experienceSlider.Step = Mathf.Pow(f: 10, p: index);
            };
            
            var enableItemMultiplierToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableItemMultiplierToggle");
            if (enableItemMultiplierToggle == null)
            {
                return;
            }
            enableItemMultiplierToggle.Checked = InstantFishingConfig.EnableItemMultiplier.Value;
            enableItemMultiplierToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableItemMultiplier.Value = isChecked;
            };
            
            var itemSlider = GetRequiredPreBuild<OptSlider>(builder: builder, id: "itemSlider");
            if (itemSlider == null)
            {
                return;
            }
            itemSlider.Title = InstantFishingConfig.ItemMultiplier.Value.ToString();
            itemSlider.Value = InstantFishingConfig.ItemMultiplier.Value;
            itemSlider.Step = 1;
            itemSlider.OnValueChanged += value =>
            {
                itemSlider.Title = value.ToString();
                InstantFishingConfig.ItemMultiplier.Value = (int)value;
            };
            
            var enableSetTierToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableSetTierToggle");
            if (enableSetTierToggle == null)
            {
                return;
            }
            enableSetTierToggle.Checked = InstantFishingConfig.EnableSetTier.Value;
            enableSetTierToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableSetTier.Value = isChecked;
            };
            
            var setTierDropdown = GetRequiredPreBuild<OptDropdown>(builder: builder, id: "setTierDropdown");
            if (setTierDropdown == null)
            {
                return;
            }
            setTierDropdown.Value = InstantFishingConfig.SetTier.Value;
            setTierDropdown.OnValueChanged += index =>
            {
                InstantFishingConfig.SetTier.Value = index;
            };
            
            var enableItemBlessedStateToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableItemBlessedStateToggle");
            if (enableItemBlessedStateToggle == null)
            {
                return;
            }
            enableItemBlessedStateToggle.Checked = InstantFishingConfig.EnableItemBlessedState.Value;
            enableItemBlessedStateToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableItemBlessedState.Value = isChecked;
            };
            
            var itemBlessedStateDropdownMapping = new Dictionary<int, int>
            {
                { 0, -2 }, // Doomed -> -2
                { 1, -1 }, // Cursed -> -1
                { 2, 0 }, // Normal -> 0
                { 3, 1 } // Blessed -> 1
            };
            
            var reverseDropdownMapping = itemBlessedStateDropdownMapping.ToDictionary(keySelector: kv => kv.Value, elementSelector: kv => kv.Key);
            
            var itemBlessedStateDropdown = GetRequiredPreBuild<OptDropdown>(builder: builder, id: "itemBlessedStateDropdown");
            if (itemBlessedStateDropdown == null)
            {
                return;
            }
            if (reverseDropdownMapping.TryGetValue(key: (int)InstantFishingConfig.ItemBlessedState.Value, value: out int dropdownIndex))
            {
                itemBlessedStateDropdown.Value = dropdownIndex;
            }
            itemBlessedStateDropdown.OnValueChanged += index =>
            {
                if (itemBlessedStateDropdownMapping.TryGetValue(key: index, value: out int mappedValue))
                {
                    InstantFishingConfig.ItemBlessedState.Value = (BlessedState)mappedValue;
                }
            };
            
            var enableZeroWeightToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableZeroWeightToggle");
            if (enableZeroWeightToggle == null)
            {
                return;
            }
            enableZeroWeightToggle.Checked = InstantFishingConfig.EnableZeroWeight.Value;
            enableZeroWeightToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableZeroWeight.Value = isChecked;
            };
            
            // QoL
            var enableAutoFishToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableAutoFishToggle");
            if (enableAutoFishToggle == null)
            {
                return;
            }
            enableAutoFishToggle.Checked = InstantFishingConfig.EnableAutoFish.Value;
            enableAutoFishToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableAutoFish.Value = isChecked;
            };
            
            var enableTurboModeToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableTurboModeToggle");
            if (enableTurboModeToggle == null)
            {
                return;
            }
            enableTurboModeToggle.Checked = InstantFishingConfig.EnableTurboMode.Value;
            enableTurboModeToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableTurboMode.Value = isChecked;
                
                if (isChecked == false)
                {
                    InstantFishing.DisableTurboAndFlushRoundTimers();
                }
            };
            
            var turboModeSlider = GetRequiredPreBuild<OptSlider>(builder: builder, id: "turboModeSlider");
            if (turboModeSlider == null)
            {
                return;
            }
            turboModeSlider.Title = InstantFishingConfig.TurboModeSpeedMultiplier.Value.ToString();
            turboModeSlider.Value = InstantFishingConfig.TurboModeSpeedMultiplier.Value;
            turboModeSlider.Step = 1;
            turboModeSlider.OnValueChanged += value =>
            {
                turboModeSlider.Title = value.ToString();
                InstantFishingConfig.TurboModeSpeedMultiplier.Value = (int)value;
            };
            
            var turboModeDropdown = GetRequiredPreBuild<OptDropdown>(builder: builder, id: "turboModeDropdown");
            if (turboModeDropdown == null)
            {
                return;
            }
            turboModeDropdown.OnValueChanged += index =>
            {
                turboModeSlider.Step = Mathf.Pow(f: 10, p: index);
            };
            
            var enableStaminaThresholdToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableStaminaThresholdToggle");
            if (enableStaminaThresholdToggle == null)
            {
                return;
            }
            enableStaminaThresholdToggle.Checked = InstantFishingConfig.EnableStaminaThreshold.Value;
            enableStaminaThresholdToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableStaminaThreshold.Value = isChecked;
            };
            
            var staminaThresholdSlider = GetRequiredPreBuild<OptSlider>(builder: builder, id: "staminaThresholdSlider");
            if (staminaThresholdSlider == null)
            {
                return;
            }
            staminaThresholdSlider.Title = InstantFishingConfig.StaminaThreshold.Value.ToString();
            staminaThresholdSlider.Value = InstantFishingConfig.StaminaThreshold.Value;
            staminaThresholdSlider.Step = 1;
            staminaThresholdSlider.OnValueChanged += value =>
            {
                staminaThresholdSlider.Title = value.ToString();
                InstantFishingConfig.StaminaThreshold.Value = (int)value;
            };
            
            var staminaThresholdDropdown = GetRequiredPreBuild<OptDropdown>(builder: builder, id: "staminaThresholdDropdown");
            if (staminaThresholdDropdown == null)
            {
                return;
            }
            staminaThresholdDropdown.OnValueChanged += index =>
            {
                staminaThresholdSlider.Step = Mathf.Pow(f: 10, p: index);
            };
            
            var enableRippleEffectToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableRippleEffectToggle");
            if (enableRippleEffectToggle == null)
            {
                return;
            }
            enableRippleEffectToggle.Checked = InstantFishingConfig.EnableRippleEffect.Value;
            enableRippleEffectToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableRippleEffect.Value = isChecked;
            };
            
            var enableAnimationsToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableAnimationsToggle");
            if (enableAnimationsToggle == null)
            {
                return;
            }
            enableAnimationsToggle.Checked = InstantFishingConfig.EnableAnimations.Value;
            enableAnimationsToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableAnimations.Value = isChecked;
            };
            
            var enableSoundsToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableSoundsToggle");
            if (enableSoundsToggle == null)
            {
                return;
            }
            enableSoundsToggle.Checked = InstantFishingConfig.EnableSounds.Value;
            enableSoundsToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableSounds.Value = isChecked;
            };
            
            var enableWinterFishingToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableWinterFishingToggle");
            if (enableWinterFishingToggle == null)
            {
                return;
            }
            enableWinterFishingToggle.Checked = InstantFishingConfig.EnableWinterFishing.Value;
            enableWinterFishingToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableWinterFishing.Value = isChecked;
            };
            
            var enableAutoEatToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableAutoEatToggle");
            if (enableAutoEatToggle == null)
            {
                return;
            }
            enableAutoEatToggle.Checked = InstantFishingConfig.EnableAutoEat.Value;
            enableAutoEatToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableAutoEat.Value = isChecked;
            };
            
            var autoEatDropdown = GetRequiredPreBuild<OptDropdown>(builder: builder, id: "autoEatDropdown");
            if (autoEatDropdown == null)
            {
                return;
            }
            autoEatDropdown.Value = InstantFishingConfig.AutoEatThreshold.Value;
            autoEatDropdown.OnValueChanged += index =>
            {
                InstantFishingConfig.AutoEatThreshold.Value = index;
            };
            
            var enableAutoSleepToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableAutoSleepToggle");
            if (enableAutoSleepToggle == null)
            {
                return;
            }
            enableAutoSleepToggle.Checked = InstantFishingConfig.EnableAutoSleep.Value;
            enableAutoSleepToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableAutoSleep.Value = isChecked;
            };
            
            var autoSleepDropdown = GetRequiredPreBuild<OptDropdown>(builder: builder, id: "autoSleepDropdown");
            if (autoSleepDropdown == null)
            {
                return;
            }
            autoSleepDropdown.Value = InstantFishingConfig.AutoSleepThreshold.Value - 1;
            autoSleepDropdown.OnValueChanged += index =>
            {
                InstantFishingConfig.AutoSleepThreshold.Value = index + 1;
            };
            
            var enableInstantBonitoFlakesToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableInstantBonitoFlakesToggle");
            if (enableInstantBonitoFlakesToggle == null)
            {
                return;
            }
            enableInstantBonitoFlakesToggle.Checked = InstantFishingConfig.EnableInstantBonitoFlakes.Value;
            enableInstantBonitoFlakesToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableInstantBonitoFlakes.Value = isChecked;
            };
            
            var enableExcludeTierFishFromBonitoToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableExcludeTierFishFromBonitoToggle");
            if (enableExcludeTierFishFromBonitoToggle == null)
            {
                return;
            }
            enableExcludeTierFishFromBonitoToggle.Checked = InstantFishingConfig.EnableExcludeTierFishFromBonito.Value;
            enableExcludeTierFishFromBonitoToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableExcludeTierFishFromBonito.Value = isChecked;
            };
            
            var enableInstantWineToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableInstantWineToggle");
            if (enableInstantWineToggle == null)
            {
                return;
            }
            enableInstantWineToggle.Checked = InstantFishingConfig.EnableInstantWine.Value;
            enableInstantWineToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableInstantWine.Value = isChecked;
            };
            
            var enableAutoDumpToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableAutoDumpToggle");
            if (enableAutoDumpToggle == null)
            {
                return;
            }
            enableAutoDumpToggle.Checked = InstantFishingConfig.EnableAutoDump.Value;
            enableAutoDumpToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.EnableAutoDump.Value = isChecked;
            };

            var autoDumpTriggerDropdown = GetRequiredPreBuild<OptDropdown>(builder: builder, id: "autoDumpTriggerDropdown");
            if (autoDumpTriggerDropdown == null)
            {
                return;
            }
            autoDumpTriggerDropdown.Value = (int)InstantFishingConfig.AutoDumpTrigger.Value;
            autoDumpTriggerDropdown.OnValueChanged += index =>
            {
                InstantFishingConfig.AutoDumpTrigger.Value = (AutoDumpTriggerMode)index;
            };

            var autoDumpThresholdSlider = GetRequiredPreBuild<OptSlider>(builder: builder, id: "autoDumpThresholdSlider");
            if (autoDumpThresholdSlider == null)
            {
                return;
            }
            autoDumpThresholdSlider.Title = GetAutoDumpThresholdTitle(controller: controller, value: InstantFishingConfig.AutoDumpThreshold.Value);
            autoDumpThresholdSlider.Value = InstantFishingConfig.AutoDumpThreshold.Value;
            autoDumpThresholdSlider.Step = 1;
            autoDumpThresholdSlider.OnValueChanged += value =>
            {
                int thresholdValue = (int)value;
                autoDumpThresholdSlider.Title = GetAutoDumpThresholdTitle(controller: controller, value: thresholdValue);
                InstantFishingConfig.AutoDumpThreshold.Value = thresholdValue;
            };

            BuildFishToggleGrid(builder: builder);
        };
    }

    private static void SetTranslations(ModOptionController controller)
    {
        controller.SetTranslation(id: "topic02",
            en: "Auto Dump Trigger",
            jp: "自動ダンプ条件",
            cn: "自动倾倒条件");
        controller.SetTranslation(id: "topic03",
            en: "Auto Dump Threshold",
            jp: "自動ダンプしきい値",
            cn: "自动倾倒阈值");
        controller.SetTranslation(id: "choice10",
            en: "Full Only",
            jp: "満杯のみ",
            cn: "仅满背包");
        controller.SetTranslation(id: "choice11",
            en: "Inventory Usage %",
            jp: "所持数使用率%",
            cn: "背包占用率%");
        controller.SetTranslation(id: "choice12",
            en: "Carry Weight %",
            jp: "重量使用率%",
            cn: "负重使用率%");
    }

    private static string GetAutoDumpThresholdTitle(ModOptionController controller, int value)
    {
        return $"{controller.Tr(contentId: "topic03")}: {value}%";
    }

    private static void BuildFishToggleGrid(OptionUIBuilder builder)
    {
        var fishGridContainer = builder.GetPreBuild<OptVLayout>(id: "vlayout19");

        if (fishGridContainer == null)
        {
            return;
        }

        List<SourceThing.Row> fishRows = InstantFishingConfig.GetAvailableFishRows();

        if (fishRows.Count == 0)
        {
            return;
        }

        HashSet<string> selectedFishIds = new HashSet<string>(collection: InstantFishingConfig.SelectedFishIds);
        OptHLayout? currentRow = null;

        for (int i = 0; i < fishRows.Count; i++)
        {
            if (i % FishGridColumns == 0)
            {
                currentRow = fishGridContainer.AddHLayout();
            }

            SourceThing.Row fishRow = fishRows[index: i];
            string fishId = fishRow.id;
            string fishName = string.IsNullOrWhiteSpace(value: fishRow.GetName())
                ? fishId
                : fishRow.GetName();

            var fishToggle = currentRow!.AddToggle(
                text: fishName,
                isChecked: selectedFishIds.Contains(item: fishId));

            fishToggle.OnValueChanged += isChecked =>
            {
                InstantFishingConfig.SetFishSelected(fishId: fishId, isSelected: isChecked);
            };
        }
    }

    private static T? GetRequiredPreBuild<T>(OptionUIBuilder builder, string id) where T : OptUIElement
    {
        T? element = builder.GetPreBuild<T>(id: id);
        if (element == null)
        {
            InstantFishing.LogError(message: $"Missing Mod Options prebuilt element: {id}");
        }

        return element;
    }

}
