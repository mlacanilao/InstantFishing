using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;

namespace InstantFishing
{
    public class InstantFishingWidgetMain : OmegaWidget
    {
        public override OmegaWidget Setup(object dummy)
        {
            Window window = this.AddWindow(setting: new Window.Setting
            {
                textCaption = OmegaUI.__(ja: "Instant Fishing 設定", en: "Instant Fishing Config"),
                bound = new Rect(x: 0f, y: 0f, width: 680f, height: 500f),
                transparent = false,
                allowMove = true
            });

            try
            {
                window.AddTab(
                    idLang: OmegaUI.__(ja: "Instant Fishing 設定", en: "Instant Fishing Config"),
                    content: OmegaUI.CreatePage<InstantFishingWidgetMain.ConfigUI>(
                        id: "instantfishing.config",
                        window: window).root,
                    action: null,
                    sprite: null,
                    langTooltip: null);
            }
            catch (Exception ex)
            {
                Debug.Log(message: $"[Instant Fishing] {ex.Message}");
            }

            return this;
        }

        public class ConfigUI : OmegaLayout<object>
        {
            private ScrollLayout scrollLayout;
            public override void OnCreate(object dummy)
            {
                this.layout.childControlHeight = true;
                this.layout.childForceExpandHeight = true;

                base.AddText(
                    text: OmegaUI.__(ja: "インスタントフィッシングMODを有効または無効にします。",
                        en: "Enable or disable Instant Fishing mod."),
                    parent: this.layout.transform
                );
                
                base.AddToggle(
                    text: OmegaUI.__(ja: "Enable Instant Fishing Mod", en: "Enable Instant Fishing Mod"),
                    isOn: InstantFishingConfig.EnableInstantFishingMod.Value,
                    action: isOn =>
                    {
                        InstantFishingConfig.EnableInstantFishingMod.Value = isOn;
                        string status = isOn ? "enabled" : "disabled";
                        ELayer.pc.TalkRaw(text: $"Instant Fishing mod is now {status}.", ref1: null, ref2: null, forceSync: false);
                        ToggleFeatures(isEnabled: isOn);
                    },
                    parent: this.layout.transform
                );
                
                scrollLayout = AddScrollLayout(parent: this.layout.transform);
                scrollLayout.headerRect.SetActive(enable: true);
                scrollLayout.uiHeader.SetText(s: "Features");
                scrollLayout.layout.spacing = 10f;
                
                AddToggleWithInputTextField(
                    parent: scrollLayout.root,
                    toggleName: "Enable Hit Value",
                    inputLabel: "Hit Value",
                    jaDescription: "インスタントフィッシングのカスタムヒット値を有効または無効にします。\n" +
                                   "魚を即座に捕まえるための進行値を設定します。\n" +
                                   "値が高いほど、魚を簡単に捕まえられます。",
                    enDescription: "Enable or disable the custom hit value for instant fishing.\n" +
                                   "Set the progress value for catching fish instantly.\n" +
                                   "Higher values make it easier to catch fish instantly.",
                    toggleConfig: InstantFishingConfig.EnableHitValue,
                    inputConfig: InstantFishingConfig.HitValue,
                    minValue: 0,
                    maxValue: 100
                );
                
                AddToggleWithInputTextField(
                    parent: scrollLayout.root,
                    toggleName: "Enable Auto Bonito Flakes",
                    inputLabel: "Excluded Bonito Flakes Fish List",
                    jaDescription: "魚をかつお節フレークに自動変換する機能を有効または無効にします。\n" +
                                   "かつお節フレークへの自動変換から除外する魚の名前またはIDのピリオド区切りリストを設定します。\n" +
                                   "英語と日本語の名前をサポートしています。",
                    enDescription: "Enable or disable automatic conversion of fish to bonito flakes.\n" +
                                   "Set a period-separated list of fish names or IDs to exclude from automatic bonito flakes conversion.\n" +
                                   "Supports both English and Japanese names.",
                    toggleConfig: InstantFishingConfig.EnableAutoBonitoFlakes,
                    inputConfig: InstantFishingConfig.ExcludedBonitoFlakesFishList
                );
                
                AddEnableToggleWithDescription(
                    parent: scrollLayout.root,
                    toggleName: "Enable Auto Wine",
                    jaDescription: "魚とかつお節を自動でワインに熟成する機能を有効または無効にします。\n" +
                                   "'true' に設定すると魚とかつお節が自動的にワインに変換され、'false' に設定すると無効になります。",
                    enDescription: "Enable or disable automatic aging of fish and bonito flakes into wine.\n" +
                                   "Set to 'true' to automatically convert fish and bonito flakes into wine, or 'false' to disable the feature.",
                    configEntry: InstantFishingConfig.EnableAutoWine
                );
                
                AddToggleWithInputTextField(
                    parent: scrollLayout.root,
                    toggleName: "Enable Turbo",
                    inputLabel: "Turbo Speed",
                    jaDescription: "釣りのターボモードを有効または無効にします。\n" +
                                   "釣り中のターボモードのスピード倍率を設定します。\n" +
                                   "（例: 倍速の場合は2）",
                    enDescription: "Enable or disable Turbo Mode for faster fishing.\n" +
                                   "Set the speed multiplier for Turbo Mode during fishing.\n" +
                                   "(e.g., 2 for double speed)",
                    toggleConfig: InstantFishingConfig.EnableTurboMode,
                    inputConfig: InstantFishingConfig.TurboSpeed,
                    minValue: 0,
                    maxValue: 100
                );

                AddToggleWithInputTextField(
                    parent: scrollLayout.root,
                    toggleName: "Enable Stamina Check",
                    inputLabel: "Stamina Threshold",
                    jaDescription: "釣り中のスタミナチェックを有効または無効にします。\n" +
                                   "釣りを続けるために必要な最小スタミナを設定します。\n" +
                                   "所有者のスタミナがこの値を下回ると、釣りアクションがキャンセルされます。",
                    enDescription: "Enable or disable the stamina check during fishing.\n" +
                                   "Set the minimum stamina required to continue fishing.\n" +
                                   "If the player's stamina drops below this value, the fishing action will cancel.",
                    toggleConfig: InstantFishingConfig.EnableStaminaCheck,
                    inputConfig: InstantFishingConfig.StaminaThreshold,
                    minValue: -9999,
                    maxValue: BaseStats.CC._maxStamina * BaseStats.CC.Evalue(ele: 62) / 100
                );
                
                AddEnableToggleWithDescription(parent: scrollLayout.root, 
                    toggleName: "Enable Ripple Effect", 
                    jaDescription: "釣り中のリップルエフェクトを有効または無効にします。\n" +
                                   "'true' に設定すると釣り中にリップルアニメーションと効果音が再生され、'false' に設定すると無効になります。",
                    enDescription: "Enable or disable the ripple effect during fishing.\n" +
                                   "Set to 'true' to play ripple animations and sound effects when fishing, or 'false' to disable them.",
                    configEntry: InstantFishingConfig.EnableRipple
                );
                
                AddToggleWithInputTextField(
                    parent: scrollLayout.root,
                    toggleName: "Enable Item Multiplier",
                    inputLabel: "Item Multiplier",
                    jaDescription: "釣り中に捕まえるアイテム数の倍率を有効または無効にします。\n" +
                                   "捕まえるアイテム数の倍率を設定します。\n" +
                                   "整数値である必要があります（例: 2 はアイテムが2倍になります）。",
                    enDescription: "Enable or disable the item multiplier during fishing.\n" +
                                   "Set the multiplier for the number of items caught during fishing.\n" +
                                   "Must be an integer value (e.g., 2 for double items).",
                    toggleConfig: InstantFishingConfig.EnableItemMultiplier,
                    inputConfig: InstantFishingConfig.ItemMultiplier,
                    minValue: 1,
                    maxValue: 100
                );
                
                AddToggleWithDropdown(
                    parent: scrollLayout.root,
                    toggleName: "Enable Item Blessed State",
                    jaDescription: "アイテムの祝福状態を設定するかどうかを有効または無効にします。\n" +
                                   "アイテムを祝福状態または通常状態にするかを選択します。\n" +
                                   "'true' に設定すると祝福されたアイテムになり、'false' に設定すると通常のアイテムになります。",
                    enDescription: "Enable or disable setting blessed state for items.\n" +
                                   "Determine whether items should be blessed or normal.\n" +
                                   "Set to 'true' for blessed items, or 'false' for normal items.\n",
                    toggleConfig: InstantFishingConfig.EnableItemBlessedState,
                    dropdownConfig: InstantFishingConfig.IsBlessed,
                    jaDropdownLabel: "Is Blessed",
                    enDropdownLabel: "Is Blessed"
                );
                
                AddEnableToggleWithDescription(
                    parent: scrollLayout.root,
                    toggleName: "Enable Zero Weight",
                    jaDescription: "アイテムの重量をゼロに設定する機能を有効または無効にします。\n" +
                                   "'true' に設定すると捕まえたアイテムの重量がゼロになり、'false' に設定すると元の重量が保持されます。",
                    enDescription: "Enable or disable setting item weight to zero.\n" +
                                   "Set to 'true' to change the weight of caught items to zero, or 'false' to retain the original weight.",
                    configEntry: InstantFishingConfig.EnableZeroWeight
                );

                ToggleFeatures(isEnabled: InstantFishingConfig.EnableInstantFishingMod.Value);
            }

            private void AddEnableToggleWithDescription(Transform parent, string toggleName, string jaDescription, string enDescription, ConfigEntry<bool> configEntry)
            {
                base.AddText(
                    text: OmegaUI.__(ja: jaDescription, en: enDescription),
                    parent: parent
                );

                base.AddToggle(
                    text: OmegaUI.__(ja: toggleName, en: toggleName),
                    isOn: configEntry.Value,
                    action: isOn =>
                    {
                        configEntry.Value = isOn;
                        string status = isOn ? "enabled" : "disabled";
                        ELayer.pc.TalkRaw(text: $"{toggleName} is now {status}.", ref1: null, ref2: null, forceSync: false);
                    },
                    parent: parent
                );
            }
            
            private void AddToggleWithDropdown(Transform parent, string toggleName, string jaDescription, string enDescription, ConfigEntry<bool> toggleConfig, ConfigEntry<bool> dropdownConfig, string jaDropdownLabel, string enDropdownLabel)
            {
                base.AddText(
                    text: OmegaUI.__(ja: jaDescription, en: enDescription),
                    parent: parent
                );

                OmegaLayout<object>.LayoutGroup layoutGroup = null;
                CanvasGroup canvasGroup = null;
                
                base.AddToggle(
                    text: OmegaUI.__(ja: toggleName, en: toggleName),
                    isOn: toggleConfig.Value,
                    action: isOn =>
                    {
                        toggleConfig.Value = isOn;
                        string status = isOn ? "enabled" : "disabled";
                        ELayer.pc.TalkRaw(text: $"{toggleName} is now {status}.", ref1: null, ref2: null, forceSync: false);
                        ToggleLayout(isEnabled: isOn);
                    },
                    parent: parent
                );
                
                // Create a layout group for the dropdown (initially hidden)
                layoutGroup = base.AddLayoutGroup(parent: parent);
                layoutGroup.group.childControlWidth = false;
                layoutGroup.group.childForceExpandWidth = false;
                
                canvasGroup = layoutGroup.ui.gameObject.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = layoutGroup.ui.gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.interactable = toggleConfig.Value;
                canvasGroup.blocksRaycasts = toggleConfig.Value;
                canvasGroup.alpha = toggleConfig.Value ? 1f : 0.5f;

                // Add the dropdown description
                base.AddText(
                    text: OmegaUI.__(ja: jaDropdownLabel, en: enDropdownLabel),
                    parent: layoutGroup.transform
                );

                // Add the dropdown
                UIDropdown uidropdown = base.AddDropdown(parent: layoutGroup.transform);
                uidropdown.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Horizontal, size: 150f);

                // Configure dropdown options
                uidropdown.ClearOptions();
                uidropdown.options.Add(item: new Dropdown.OptionData(text: "True"));
                uidropdown.options.Add(item: new Dropdown.OptionData(text: "False"));

                // Set initial value based on the dropdown config
                uidropdown.value = dropdownConfig.Value ? 0 : 1;

                // Add listener to update the dropdown config
                uidropdown.onValueChanged.AddListener(call: delegate (int i)
                {
                    dropdownConfig.Value = i == 0; // True if index is 0, False if index is 1
                    ELayer.pc.TalkRaw(
                        text: $"{enDropdownLabel} is now {(dropdownConfig.Value ? "enabled" : "disabled")}.",
                        ref1: null, ref2: null, forceSync: false
                    );
                });
                
                void ToggleLayout(bool isEnabled)
                {
                    if (layoutGroup != null)
                    {
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = isEnabled;
                            canvasGroup.blocksRaycasts = isEnabled;
                            canvasGroup.alpha = isEnabled ? 1f : 0.5f;
                        }
                    }
                }
            }
            
            private void AddToggleWithInputTextField(Transform parent, string toggleName, string inputLabel, string jaDescription, string enDescription, ConfigEntry<bool> toggleConfig, ConfigEntry<int> inputConfig, int minValue, int maxValue)
            {
                // Add the description for the toggle and input field
                base.AddText(
                    text: OmegaUI.__(ja: jaDescription, en: enDescription),
                    parent: parent
                );

                OmegaLayout<object>.LayoutGroup layoutGroup = null;
                CanvasGroup canvasGroup = null;

                // Add the toggle
                base.AddToggle(
                    text: OmegaUI.__(ja: toggleName, en: toggleName),
                    isOn: toggleConfig.Value,
                    action: isOn =>
                    {
                        toggleConfig.Value = isOn;
                        string status = isOn ? "enabled" : "disabled";
                        ELayer.pc.TalkRaw(text: $"{toggleName} is now {status}.", ref1: null, ref2: null, forceSync: false);
                        ToggleLayout(isEnabled: isOn);
                    },
                    parent: parent
                );

                // Create a layout group for the input field (initially hidden)
                layoutGroup = base.AddLayoutGroup(parent: parent);
                layoutGroup.group.childControlWidth = false;
                layoutGroup.group.childForceExpandWidth = false;
                
                canvasGroup = layoutGroup.ui.gameObject.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = layoutGroup.ui.gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.interactable = toggleConfig.Value;
                canvasGroup.blocksRaycasts = toggleConfig.Value;
                canvasGroup.alpha = toggleConfig.Value ? 1f : 0.5f;

                // Add the input label
                base.AddText(
                    text: OmegaUI.__(ja: inputLabel, en: inputLabel),
                    parent: layoutGroup.transform
                );

                // Add the input text field
                var inputTextField = base.AddInputText(parent: layoutGroup.group.transform);
                inputTextField.input.Num = inputConfig.Value; // Set initial value
                inputTextField.transform.SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Horizontal, size: 100f);
                inputTextField.inputTransform.SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Horizontal, size: 100f);
                inputTextField.placeholder.SetActive(enable: true);
                inputTextField.placeholderText.text = OmegaUI.__(ja: "上書き", en: "modify");

                // Add value change listener with clamping
                inputTextField.input.onValueChanged = value =>
                {
                    int clampedValue = Mathf.Clamp(value: value, min: minValue, max: maxValue); // Clamp the value between min and max
                    inputConfig.Value = clampedValue;
                    inputTextField.input.Num = clampedValue; // Update the input field with the clamped value
                    ELayer.pc.TalkRaw(
                        text: $"{inputLabel} set to {clampedValue}.",
                        ref1: null, ref2: null, forceSync: false
                    );
                };

                void ToggleLayout(bool isEnabled)
                {
                    if (layoutGroup != null)
                    {
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = isEnabled;
                            canvasGroup.blocksRaycasts = isEnabled;
                            canvasGroup.alpha = isEnabled ? 1f : 0.5f;
                        }
                    }
                }
            }
            
            private void AddToggleWithInputTextField(
                Transform parent, 
                string toggleName, 
                string inputLabel, 
                string jaDescription, 
                string enDescription, 
                ConfigEntry<bool> toggleConfig, 
                ConfigEntry<string> inputConfig)
            {
                // Add the description for the toggle and input field
                base.AddText(
                    text: OmegaUI.__(ja: jaDescription, en: enDescription),
                    parent: parent
                );

                OmegaLayout<object>.LayoutGroup layoutGroup = null;
                CanvasGroup canvasGroup = null;

                // Add the toggle
                base.AddToggle(
                    text: OmegaUI.__(ja: toggleName, en: toggleName),
                    isOn: toggleConfig.Value,
                    action: isOn =>
                    {
                        toggleConfig.Value = isOn;
                        string status = isOn ? "enabled" : "disabled";
                        ELayer.pc.TalkRaw(text: $"{toggleName} is now {status}.", ref1: null, ref2: null, forceSync: false);
                        ToggleLayout(isEnabled: isOn);
                    },
                    parent: parent
                );

                // Create a layout group for the input field (initially hidden)
                layoutGroup = base.AddLayoutGroup(parent: parent);
                layoutGroup.group.childControlWidth = false;
                layoutGroup.group.childForceExpandWidth = false;

                canvasGroup = layoutGroup.ui.gameObject.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = layoutGroup.ui.gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.interactable = toggleConfig.Value;
                canvasGroup.blocksRaycasts = toggleConfig.Value;
                canvasGroup.alpha = toggleConfig.Value ? 1f : 0.5f;

                // Add the input label
                base.AddText(
                    text: OmegaUI.__(ja: inputLabel, en: inputLabel),
                    parent: layoutGroup.transform
                );

                // Add the input text field
                var inputTextField = base.AddInputText(parent: layoutGroup.group.transform);
                inputTextField.input.type = UIInputText.Type.Name;
                inputTextField.input.field.characterLimit = 4096;
                inputTextField.input.Text = inputConfig.Value;
                inputTextField.transform.SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Horizontal, size: 200f);
                inputTextField.inputTransform.SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Horizontal, size: 200f);
                inputTextField.placeholder.SetActive(enable: true);
                inputTextField.placeholderText.text = OmegaUI.__(ja: "上書き", en: "modify");

                // Add value change listener
                inputTextField.input.onValueChanged = value =>
                {
                    string stringValue = inputTextField.input.Text;
                    inputConfig.Value = stringValue; // Update the config with the new string value
                    ELayer.pc.TalkRaw(
                        text: $"{inputLabel} set to \"{stringValue}\".",
                        ref1: null, ref2: null, forceSync: false
                    );
                };
                
                void ToggleLayout(bool isEnabled)
                {
                    if (layoutGroup != null)
                    {
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = isEnabled;
                            canvasGroup.blocksRaycasts = isEnabled;
                            canvasGroup.alpha = isEnabled ? 1f : 0.5f;
                        }
                    }
                }
            }
            
            private void ToggleFeatures(bool isEnabled)
            {
                if (scrollLayout != null)
                {
                    var canvasGroup = scrollLayout.root.gameObject.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        canvasGroup = scrollLayout.root.gameObject.AddComponent<CanvasGroup>();
                    }
                    canvasGroup.interactable = isEnabled;
                    canvasGroup.blocksRaycasts = isEnabled;
                    canvasGroup.alpha = isEnabled ? 1f : 0.5f;
                }
            }
        }
    }
}