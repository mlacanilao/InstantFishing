using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Configuration;

namespace InstantFishing.Config
{
    internal static class InstantFishingConfig
    {
        // Cheats
        internal static ConfigEntry<bool> EnableInstantFishing;
        internal static ConfigEntry<bool> EnableItemMultiplier;
        internal static ConfigEntry<int> ItemMultiplier;
        internal static ConfigEntry<bool> EnableItemBlessedState;
        internal static ConfigEntry<BlessedState> ItemBlessedState;
        internal static ConfigEntry<bool> EnableExperienceMultiplier;
        internal static ConfigEntry<int> ExperienceMultiplier;
        internal static ConfigEntry<bool> EnableZeroWeight;
        
        // QoL
        internal static ConfigEntry<bool> EnableAutoFish;
        internal static ConfigEntry<bool> EnableTurboMode;
        internal static ConfigEntry<int> TurboModeSpeedMultiplier;
        internal static ConfigEntry<bool> EnableStaminaThreshold;
        internal static ConfigEntry<int> StaminaThreshold;
        internal static ConfigEntry<bool> EnableRippleEffect;
        internal static ConfigEntry<bool> EnableAnimations;
        internal static ConfigEntry<bool> EnableSounds;
        internal static ConfigEntry<bool> EnableWinterFishing;
        internal static ConfigEntry<bool> EnableAutoEat;
        internal static ConfigEntry<int> AutoEatThreshold;
        internal static ConfigEntry<bool> EnableAutoSleep;
        internal static ConfigEntry<int> AutoSleepThreshold;
        internal static ConfigEntry<bool> EnableAutoDump;
        
        internal static string XmlPath { get; private set; }
        internal static string TranslationXlsxPath { get; private set; }


        internal static void LoadConfig(ConfigFile config)
        {
            EnableInstantFishing = config.Bind(
                section: ModInfo.Name,
                key: "Enable Instant Fishing",
                defaultValue: true,
                description: "Enable or disable instant fishing.\n" +
                             "Set to 'true' to activate it, or 'false' to keep it disabled.\n" +
                             "インスタントフィッシングを有効または無効にします。\n" +
                             "'true' に設定すると有効になり、'false' に設定すると無効になります。\n" +
                             "启用或禁用瞬间钓鱼。\n" +
                             "设置为 'true' 即启用，设置为 'false' 即禁用。");
            
            EnableExperienceMultiplier = config.Bind(
                section: ModInfo.Name,
                key: "Enable Experience Multiplier",
                defaultValue: false,
                description: "Enable or disable the fishing experience multiplier.\n" +
                             "Set to 'true' to use a multiplier for fishing experience, or 'false' to disable it.\n" +
                             "釣り経験値の倍率機能を有効または無効にします。\n" +
                             "'true' に設定すると釣り経験値の倍率が有効になり、'false' に設定すると無効になります。\n" +
                             "启用或禁用钓鱼经验值倍增功能。\n" +
                             "设置为 'true' 使用经验值倍增器，设置为 'false' 表示禁用。"
            );
            
            ExperienceMultiplier = config.Bind(
                section: ModInfo.Name,
                key: "Experience Multiplier",
                defaultValue: 1,
                description: "Set the multiplier for the amount of fishing experience gained.\n" +
                             "Must be an integer value (e.g., 2 for double experience).\n" +
                             "釣り中に得られる経験値の倍率を設定します。\n" +
                             "整数値である必要があります（例: 2 は経験値が2倍になります）。\n" +
                             "设置钓鱼时获得经验值的倍增器。\n" +
                             "必须为整数值（例如，2 表示经验值翻倍）。"
            );
            
            EnableItemMultiplier = config.Bind(
                section: ModInfo.Name,
                key: "Enable Item Multiplier",
                defaultValue: false,
                description: "Enable or disable the item multiplier during fishing.\n" +
                             "Set to 'true' to activate item multiplier logic, or 'false' to disable it.\n" +
                             "釣り中のアイテム倍率を有効または無効にします。\n" +
                             "'true' に設定するとアイテム倍率ロジックが有効になり、'false' に設定すると無効になります。\n" +
                             "启用或禁用钓鱼时的物品倍增器。\n" +
                             "设置为 'true' 启用物品倍增逻辑，设置为 'false' 禁用。"
            );
            
            ItemMultiplier = config.Bind(
                section: ModInfo.Name,
                key: "Item Multiplier",
                defaultValue: 1,
                description: "Set the multiplier for the number of items caught during fishing.\n" +
                             "Higher values increase the number of items caught.\n" +
                             "Must be an integer value (e.g., 2 for double items).\n" +
                             "釣り中に捕まえるアイテム数の倍率を設定します。\n" +
                             "値が高いほど、捕まえるアイテム数が増加します。\n" +
                             "整数値である必要があります（例: 2 はアイテムが2倍になります）。\n" +
                             "设置钓鱼时获得物品数量的倍增器。\n" +
                             "值越高，捕获的物品数量越多。\n" +
                             "必须为整数值（例如，2 表示物品数量翻倍）。"
            );
            
            EnableItemBlessedState = config.Bind(
                section: ModInfo.Name,
                key: "Enable Item Blessed State",
                defaultValue: false,
                description: "Enable or disable setting blessed state for items.\n" +
                             "Set to 'true' to allow items to have a blessed state, or 'false' to disable it.\n" +
                             "アイテムにカスタム祝福状態を設定するかどうかを有効または無効にします。\n" +
                             "'true' に設定するとカスタム祝福状態が許可され、'false' に設定すると無効になります。\n" +
                             "启用或禁用物品的祝福状态设置。\n" +
                             "设置为 'true' 允许物品拥有祝福状态，设置为 'false' 禁用此功能。"
            );
            
            ItemBlessedState = config.Bind(
                section: ModInfo.Name,
                key: "Item Blessed State",
                defaultValue: BlessedState.Normal,
                description: "Set the blessed state for items.\n" +
                             "Available states are Doomed, Cursed, Normal, and Blessed.\n" +
                             "アイテムの祝福状態を設定します。\n" +
                             "利用可能な状態は、呪われた (Doomed)、軽い呪い (Cursed)、通常 (Normal)、祝福 (Blessed) です。\n" +
                             "设置物品的祝福状态。\n" +
                             "可用状态包括厄运 (Doomed)、诅咒 (Cursed)、正常 (Normal)、祝福 (Blessed)。"
            );
            
            EnableAutoFish = config.Bind(
                section: ModInfo.Name,
                key: "Enable Auto Fish",
                defaultValue: false,
                description: "Enable or disable automatic fishing. Requires 'Enable Auto Dump' or 'Enable Auto Sleep' to be enabled.\n" +
                             "Set to 'true' to automatically start fishing after auto dump or auto sleep, or 'false' to disable it.\n" +
                             "自動釣りを有効または無効にします。「自動ダンプを有効化」または「自動睡眠を有効化」を有効にする必要があります。\n" +
                             "'true' に設定すると、自動ダンプまたは自動睡眠の後で自動で釣りを開始し、'false' に設定すると無効になります。\n" +
                             "启用或禁用自动钓鱼功能。需要启用“启用自动倾倒”或“启用自动睡眠”。\n" +
                             "设置为 'true' 可在自动倾倒或自动睡眠后自动开始钓鱼，设置为 'false' 则禁用此功能。"
            );
            
            EnableTurboMode = config.Bind(
                section: ModInfo.Name,
                key: "Enable Turbo Mode",
                defaultValue: false,
                description: "Enable or disable turbo mode for faster fishing.\n" +
                             "Set to 'true' to activate Turbo Mode, or 'false' to disable it.\n" +
                             "釣りのターボモードを有効または無効にします。\n" +
                             "'true' に設定するとターボモードが有効になり、'false' に設定すると無効になります。\n" +
                             "启用或禁用快速钓鱼的涡轮模式。\n" +
                             "设置为 'true' 启用涡轮模式，设置为 'false' 禁用。"
            );

            TurboModeSpeedMultiplier = config.Bind(
                section: ModInfo.Name,
                key: "Turbo Mode Speed Multiplier",
                defaultValue: 1,
                description: "Set the speed multiplier for turbo mode during fishing.\n" +
                             "Must be an integer value (e.g., 2 for double speed).\n" +
                             "釣りのターボモード中のスピード倍率を設定します。\n" +
                             "整数値である必要があります（例: 倍速の場合は2）。\n" +
                             "设置钓鱼涡轮模式下的速度倍增器。\n" +
                             "必须为整数值（例如，2 表示双倍速度）。"
            );
            
            EnableZeroWeight = config.Bind(
                section: ModInfo.Name,
                key: "Enable Zero Weight",
                defaultValue: false,
                description: "Enable or disable setting item weight to zero.\n" +
                             "Set to 'true' to change the weight of caught items to zero, or 'false' to retain the original weight.\n" +
                             "アイテムの重量をゼロに設定する機能を有効または無効にします。\n" +
                             "'true' に設定すると捕まえたアイテムの重量がゼロになり、'false' に設定すると元の重量が保持されます。\n" +
                             "启用或禁用将物品重量设置为零。\n" +
                             "设置为 'true' 将捕获物品的重量更改为零，设置为 'false' 保留原始重量。"
            );
            
            EnableStaminaThreshold = config.Bind(
                section: ModInfo.Name,
                key: "Enable Stamina Threshold",
                defaultValue: true,
                description: "Enable or disable the stamina threshold check during fishing.\n" +
                             "Set to 'true' to activate the stamina threshold check, or 'false' to skip it.\n" +
                             "釣り中のスタミナチェックを有効または無効にします。\n" +
                             "'true' に設定するとスタミナチェックが有効になり、'false' に設定すると無効になります。\n" +
                             "启用或禁用钓鱼时的耐力阈值检查。\n" +
                             "设置为 'true' 激活耐力阈值检查，设置为 'false' 跳过检查。"
            );

            StaminaThreshold = config.Bind(
                section: ModInfo.Name,
                key: "Stamina Threshold",
                defaultValue: 0,
                description: "Set the minimum stamina required to continue fishing.\n" +
                             "If the player's stamina drops below this value, the fishing action will cancel.\n" +
                             "釣りを続けるために必要な最小スタミナを設定します。\n" +
                             "所有者のスタミナがこの値を下回ると、釣りアクションがキャンセルされます。\n" +
                             "设置继续钓鱼所需的最低耐力值。\n" +
                             "如果玩家的耐力低于此值，钓鱼动作将被取消。"
            );
            
            EnableRippleEffect = config.Bind(
                section: ModInfo.Name,
                key: "Enable Ripple Effect",
                defaultValue: true,
                description: "Enable or disable the ripple effect during fishing.\n" +
                             "Set to 'true' to play ripple animations and sound effects when fishing, or 'false' to disable them.\n" +
                             "釣り中のリップルエフェクトを有効または無効にします。\n" +
                             "'true' に設定すると釣り中にリップルアニメーションと効果音が再生され、'false' に設定すると無効になります。");
            
            EnableAnimations = config.Bind(
                section: ModInfo.Name,
                key: "Enable Animations",
                defaultValue: true,
                description: "Enable or disable animations during fishing.\n" +
                             "Set to 'true' to allow animations, or 'false' to disable them.\n" +
                             "釣り中のアニメーションを有効または無効にします。\n" +
                             "'true' に設定するとアニメーションが有効になり、'false' に設定すると無効になります。\n" +
                             "启用或禁用钓鱼时的动画。\n" +
                             "设置为 'true' 启用动画，设置为 'false' 禁用动画。"
            );
            
            EnableSounds = config.Bind(
                section: ModInfo.Name,
                key: "Enable Sounds",
                defaultValue: true,
                description: "Enable or disable sound effects during fishing.\n" +
                             "Set to 'true' to allow sound effects, or 'false' to disable them.\n" +
                             "釣り中のサウンドエフェクトを有効または無効にします。\n" +
                             "'true' に設定するとサウンドエフェクトが有効になり、'false' に設定すると無効になります。\n" +
                             "启用或禁用钓鱼时的声音效果。\n" +
                             "设置为 'true' 启用声音效果，设置为 'false' 禁用声音效果。"
            );
            
            EnableWinterFishing = config.Bind(
                section: ModInfo.Name,
                key: "Enable Winter Fishing",
                defaultValue: false,
                description: "Enable or disable fishing during winter or snow-covered areas.\n" +
                             "Set to 'true' to allow fishing even in snow-covered areas, or 'false' to restrict it.\n" +
                             "雪や冬の間の釣りを有効または無効にします。\n" +
                             "'true' に設定すると雪に覆われた地域でも釣りが可能になり、'false' に設定すると制限されます。\n" +
                             "启用或禁用在冬季或被雪覆盖区域的钓鱼。\n" +
                             "设置为 'true' 可在被雪覆盖的区域钓鱼，设置为 'false' 将限制钓鱼。"
            );
            
            EnableAutoEat = config.Bind(
                section: ModInfo.Name,
                key: "Enable Auto Eat",
                defaultValue: true,
                description: "Enable or disable automatic eating when hunger threshold is reached.\n" +
                             "Set to 'true' to automatically consume food, or 'false' to disable this feature.\n" +
                             "空腹状態が一定のしきい値に達したときに自動で食事を行う機能を有効または無効にします。\n" +
                             "'true' に設定すると自動で食べ物を消費し、'false' に設定すると無効になります。\n" +
                             "启用或禁用在饥饿阈值达到时自动进食。\n" +
                             "设置为 'true' 自动消耗食物，设置为 'false' 禁用此功能。"
            );
            
            AutoEatThreshold = config.Bind(
                section: ModInfo.Name,
                key: "Auto Eat Threshold",
                defaultValue: 3,
                description: "Set the hunger level at which automatic eating is triggered.\n" +
                             "Available levels are: 0 (Bloated), 1 (Filled), 2 (Normal), 3 (Hungry), 4 (Very Hungry), 5 (Starving).\n" +
                             "自動で食事を行う空腹レベルを設定します。\n" +
                             "利用可能なレベルは: 0 (満腹), 1 (満たされた), 2 (通常), 3 (空腹), 4 (非常に空腹), 5 (飢餓) です。\n" +
                             "设置触发自动进食的饥饿等级。\n" +
                             "可用等级包括: 0 (饱胀), 1 (饱足), 2 (正常), 3 (饥饿), 4 (非常饥饿), 5 (饥荒)。"
            );
            
            EnableAutoSleep = config.Bind(
                section: ModInfo.Name,
                key: "Enable Auto Sleep",
                defaultValue: false,
                description: "Enable or disable automatic sleeping when the sleep threshold is reached.\n" +
                             "Set to 'true' to allow automatic sleeping, or 'false' to disable it.\n" +
                             "睡眠しきい値に達した場合に自動で睡眠を行う機能を有効または無効にします。\n" +
                             "'true' に設定すると自動で睡眠を行い、'false' に設定すると無効になります。\n" +
                             "当达到睡眠阈值时启用或禁用自动睡眠功能。\n" +
                             "设置为 'true' 启用自动睡眠，设置为 'false' 禁用自动睡眠。"
            );
            
            AutoSleepThreshold = config.Bind(
                section: ModInfo.Name,
                key: "Auto Sleep Threshold",
                defaultValue: 0,
                description: "Set the sleepiness level at which automatic sleeping is triggered.\n" +
                             "Available levels are: 1 (Sleepy), 2 (Very Sleepy), 3 (Very Very Sleepy).\n" +
                             "自動で睡眠を行う眠気レベルを設定します。\n" +
                             "利用可能なレベルは: 1 (眠い), 2 (とても眠い), 3 (非常に眠い) です。\n" +
                             "设置触发自动睡眠的疲劳等级。\n" +
                             "可用等级包括: 1 (困倦), 2 (非常困倦), 3 (极度困倦)。"
            );
            
            EnableAutoDump = config.Bind(
                section: ModInfo.Name,
                key: "Enable Auto Dump",
                defaultValue: false,
                description: "Enable or disable automatic dumping of fishing items into containers when your inventory is full.\n" +
                             "Set to 'true' to automatically place items into the nearest valid container, or 'false' to disable it.\n" +
                             "インベントリが満杯になった場合に釣りで得たアイテムを自動的に有効なコンテナに移動する機能を有効または無効にします。\n" +
                             "'true' に設定するとアイテムが自動でコンテナに移動され、'false' に設定すると無効になります。\n" +
                             "当库存已满时启用或禁用自动将钓鱼物品放入容器。\n" +
                             "设置为 'true' 自动放置物品，设置为 'false' 禁用此功能。"
            );
        }
        
        internal static void InitializeXmlPath(string xmlPath)
        {
            if (File.Exists(path: xmlPath))
            {
                XmlPath = xmlPath;
            }
            else
            {
                XmlPath = string.Empty;
            }
        }
        
        internal static void InitializeTranslationXlsxPath(string xlsxPath)
        {
            if (File.Exists(path: xlsxPath))
            {
                TranslationXlsxPath = xlsxPath;
            }
            else
            {
                TranslationXlsxPath = string.Empty;
            }
        }
    }
}
