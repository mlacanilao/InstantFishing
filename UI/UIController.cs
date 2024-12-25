using System.Collections.Generic;
using EvilMask.Elin.ModOptions;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using EvilMask.Elin.ModOptions.UI;
using InstantFishing.Config;
using UnityEngine;

namespace InstantFishing.UI
{
    public static class UIController
    {
        public static void RegisterUI()
        {
            foreach (var obj in ModManager.ListPluginObject)
            {
                if (obj is BaseUnityPlugin plugin && plugin.Info.Metadata.GUID == ModInfo.ModOptionsGuid)
                {
                    var controller = ModOptionController.Register(guid: ModInfo.Guid, tooptipId: "mod.tooltip");
                    
                    var assemblyLocation = Path.GetDirectoryName(path: Assembly.GetExecutingAssembly().Location);
                    var xmlPath = Path.Combine(path1: assemblyLocation, path2: "InstantFishingConfig.xml");
                    InstantFishingConfig.InitializeXmlPath(xmlPath: xmlPath);
            
                    var xlsxPath = Path.Combine(path1: assemblyLocation, path2: "translations.xlsx");
                    InstantFishingConfig.InitializeTranslationXlsxPath(xlsxPath: xlsxPath);
                    
                    if (File.Exists(path: InstantFishingConfig.XmlPath))
                    {
                        using (StreamReader sr = new StreamReader(path: InstantFishingConfig.XmlPath))
                            controller.SetPreBuildWithXml(xml: sr.ReadToEnd());
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
                var hlayout = builder.GetPreBuild<OptHLayout>(id: "hlayout01");
                hlayout.Base.childForceExpandHeight = false;
                
                for (int i = 1; i <= 14; i++)
                {
                    string vlayoutId = $"vlayout{i:D2}";

                    var vlayout = builder.GetPreBuild<OptVLayout>(id: vlayoutId);
        
                    if (vlayout != null)
                    {
                        vlayout.Base.childForceExpandHeight = false;
                    }
                }
                
                var enableInstantFishingToggle = builder.GetPreBuild<OptToggle>(id: "enableInstantFishingToggle");
                enableInstantFishingToggle.Checked = InstantFishingConfig.EnableInstantFishing.Value;
                enableInstantFishingToggle.OnValueChanged += isChecked =>
                {
                    InstantFishingConfig.EnableInstantFishing.Value = isChecked;
                };
                
                // Cheats
                var enableExperienceMultiplierToggle = builder.GetPreBuild<OptToggle>(id: "enableExperienceMultiplierToggle");
                enableExperienceMultiplierToggle.Checked = InstantFishingConfig.EnableExperienceMultiplier.Value;
                enableExperienceMultiplierToggle.OnValueChanged += isChecked =>
                {
                    InstantFishingConfig.EnableExperienceMultiplier.Value = isChecked;
                };
                
                var experienceSlider = builder.GetPreBuild<OptSlider>(id: "experienceSlider");
                experienceSlider.Title = InstantFishingConfig.ExperienceMultiplier.Value.ToString();
                experienceSlider.Value = InstantFishingConfig.ExperienceMultiplier.Value;
                experienceSlider.Step = 1;
                experienceSlider.OnValueChanged += value =>
                {
                    experienceSlider.Title = value.ToString();
                    InstantFishingConfig.ExperienceMultiplier.Value = (int)value;
                };
                
                var experienceSliderDropdown = builder.GetPreBuild<OptDropdown>(id: "experienceSliderDropdown");
                experienceSliderDropdown.OnValueChanged += index =>
                {
                    experienceSlider.Step = Mathf.Pow(f: 10, p: index);
                };
                
                var enableItemMultiplierToggle = builder.GetPreBuild<OptToggle>(id: "enableItemMultiplierToggle");
                enableItemMultiplierToggle.Checked = InstantFishingConfig.EnableItemMultiplier.Value;
                enableItemMultiplierToggle.OnValueChanged += isChecked =>
                {
                    InstantFishingConfig.EnableItemMultiplier.Value = isChecked;
                };
                
                var itemSlider = builder.GetPreBuild<OptSlider>(id: "itemSlider");
                itemSlider.Title = InstantFishingConfig.ItemMultiplier.Value.ToString();
                itemSlider.Value = InstantFishingConfig.ItemMultiplier.Value;
                itemSlider.Step = 1;
                itemSlider.OnValueChanged += value =>
                {
                    itemSlider.Title = value.ToString();
                    InstantFishingConfig.ItemMultiplier.Value = (int)value;
                };
                
                var enableItemBlessedStateToggle = builder.GetPreBuild<OptToggle>(id: "enableItemBlessedStateToggle");
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
                
                var itemBlessedStateDropdown = builder.GetPreBuild<OptDropdown>(id: "itemBlessedStateDropdown");
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
                
                var enableZeroWeightToggle = builder.GetPreBuild<OptToggle>(id: "enableZeroWeightToggle");
                enableZeroWeightToggle.Checked = InstantFishingConfig.EnableZeroWeight.Value;
                enableZeroWeightToggle.OnValueChanged += isChecked =>
                {
                    InstantFishingConfig.EnableZeroWeight.Value = isChecked;
                };
                
                // QoL
                var enableAutoFishToggle = builder.GetPreBuild<OptToggle>(id: "enableAutoFishToggle");
                enableAutoFishToggle.Checked = InstantFishingConfig.EnableAutoFish.Value;
                enableAutoFishToggle.OnValueChanged += isChecked =>
                {
                    InstantFishingConfig.EnableAutoFish.Value = isChecked;
                };
                
                var enableTurboModeToggle = builder.GetPreBuild<OptToggle>(id: "enableTurboModeToggle");
                enableTurboModeToggle.Checked = InstantFishingConfig.EnableTurboMode.Value;
                enableTurboModeToggle.OnValueChanged += isChecked =>
                {
                    InstantFishingConfig.EnableTurboMode.Value = isChecked;
                };
                
                var turboModeSlider = builder.GetPreBuild<OptSlider>(id: "turboModeSlider");
                turboModeSlider.Title = InstantFishingConfig.TurboModeSpeedMultiplier.Value.ToString();
                turboModeSlider.Value = InstantFishingConfig.TurboModeSpeedMultiplier.Value;
                turboModeSlider.Step = 1;
                turboModeSlider.OnValueChanged += value =>
                {
                    turboModeSlider.Title = value.ToString();
                    InstantFishingConfig.TurboModeSpeedMultiplier.Value = (int)value;
                };
                
                var turboModeDropdown = builder.GetPreBuild<OptDropdown>(id: "turboModeDropdown");
                turboModeDropdown.OnValueChanged += index =>
                {
                    turboModeSlider.Step = Mathf.Pow(f: 10, p: index);
                };
                
                var enableStaminaThresholdToggle = builder.GetPreBuild<OptToggle>(id: "enableStaminaThresholdToggle");
                enableStaminaThresholdToggle.Checked = InstantFishingConfig.EnableStaminaThreshold.Value;
                enableStaminaThresholdToggle.OnValueChanged += isChecked =>
                {
                    InstantFishingConfig.EnableStaminaThreshold.Value = isChecked;
                };
                
                var staminaThresholdSlider = builder.GetPreBuild<OptSlider>(id: "staminaThresholdSlider");
                staminaThresholdSlider.Title = InstantFishingConfig.StaminaThreshold.Value.ToString();
                staminaThresholdSlider.Value = InstantFishingConfig.StaminaThreshold.Value;
                staminaThresholdSlider.Step = 1;
                staminaThresholdSlider.OnValueChanged += value =>
                {
                    staminaThresholdSlider.Title = value.ToString();
                    InstantFishingConfig.StaminaThreshold.Value = (int)value;
                };
                
                var staminaThresholdDropdown = builder.GetPreBuild<OptDropdown>(id: "staminaThresholdDropdown");
                staminaThresholdDropdown.OnValueChanged += index =>
                {
                    staminaThresholdSlider.Step = Mathf.Pow(f: 10, p: index);
                };
                
                var enableRippleEffectToggle = builder.GetPreBuild<OptToggle>(id: "enableRippleEffectToggle");
                enableRippleEffectToggle.Checked = InstantFishingConfig.EnableRippleEffect.Value;
                enableRippleEffectToggle.OnValueChanged += isChecked =>
                {
                    InstantFishingConfig.EnableRippleEffect.Value = isChecked;
                };
                
                var enableAnimationsToggle = builder.GetPreBuild<OptToggle>(id: "enableAnimationsToggle");
                enableAnimationsToggle.Checked = InstantFishingConfig.EnableAnimations.Value;
                enableAnimationsToggle.OnValueChanged += isChecked =>
                {
                    InstantFishingConfig.EnableAnimations.Value = isChecked;
                };
                
                var enableSoundsToggle = builder.GetPreBuild<OptToggle>(id: "enableSoundsToggle");
                enableSoundsToggle.Checked = InstantFishingConfig.EnableSounds.Value;
                enableSoundsToggle.OnValueChanged += isChecked =>
                {
                    InstantFishingConfig.EnableSounds.Value = isChecked;
                };
                
                var enableWinterFishingToggle = builder.GetPreBuild<OptToggle>(id: "enableWinterFishingToggle");
                enableWinterFishingToggle.Checked = InstantFishingConfig.EnableWinterFishing.Value;
                enableWinterFishingToggle.OnValueChanged += isChecked =>
                {
                    InstantFishingConfig.EnableWinterFishing.Value = isChecked;
                };
                
                var enableAutoEatToggle = builder.GetPreBuild<OptToggle>(id: "enableAutoEatToggle");
                enableAutoEatToggle.Checked = InstantFishingConfig.EnableAutoEat.Value;
                enableAutoEatToggle.OnValueChanged += isChecked =>
                {
                    InstantFishingConfig.EnableAutoEat.Value = isChecked;
                };
                
                var autoEatDropdown = builder.GetPreBuild<OptDropdown>(id: "autoEatDropdown");
                autoEatDropdown.Value = InstantFishingConfig.AutoEatThreshold.Value;
                autoEatDropdown.OnValueChanged += index =>
                {
                    InstantFishingConfig.AutoEatThreshold.Value = index;
                };
                
                var enableAutoSleepToggle = builder.GetPreBuild<OptToggle>(id: "enableAutoSleepToggle");
                enableAutoSleepToggle.Checked = InstantFishingConfig.EnableAutoSleep.Value;
                enableAutoSleepToggle.OnValueChanged += isChecked =>
                {
                    InstantFishingConfig.EnableAutoSleep.Value = isChecked;
                };
                
                var autoSleepDropdownMapping = new Dictionary<int, int>
                {
                    { 0, 1 }, // Sleepy -> 1
                    { 1, 2 }, // VerySleepy -> 2
                    { 2, 3 } // VeryVerySleepy -> 3
                };
                
                var autoSleepDropdown = builder.GetPreBuild<OptDropdown>(id: "autoSleepDropdown");
                autoSleepDropdown.Value = InstantFishingConfig.AutoSleepThreshold.Value - 1;
                autoSleepDropdown.OnValueChanged += index =>
                {
                    if (autoSleepDropdownMapping.TryGetValue(key: index, value: out int mappedValue))
                    {
                        InstantFishingConfig.AutoSleepThreshold.Value = mappedValue;
                    }
                };
                
                var enableAutoDumpToggle = builder.GetPreBuild<OptToggle>(id: "enableAutoDumpToggle");
                enableAutoDumpToggle.Checked = InstantFishingConfig.EnableAutoDump.Value;
                enableAutoDumpToggle.OnValueChanged += isChecked =>
                {
                    InstantFishingConfig.EnableAutoDump.Value = isChecked;
                };
            };
        }
    }
}