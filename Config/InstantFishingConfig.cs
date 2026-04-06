using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Configuration;

namespace InstantFishing.Config;

internal enum AutoDumpTriggerMode
{
    FullOnly = 0,
    InventoryUsagePercent = 1,
    CarryWeightPercent = 2
}

internal static class InstantFishingConfig
{
    private const string DefaultSelectedFishIds = "74,69,62,77,83,81,65,66,84,67,64,78,95,68,79,87,88,85,72,86,70,76,75,63,71,73,82,80";
    private static readonly HashSet<string> ExcludedFishIds = new HashSet<string>
    {
        "65_gold",
        "fish_slice",
    };

    // Cheats
    internal static ConfigEntry<bool> EnableInstantFishing = null!;
    internal static ConfigEntry<bool> EnableInstantReadAncientBook = null!;
    internal static ConfigEntry<bool> EnableItemMultiplier = null!;
    internal static ConfigEntry<int> ItemMultiplier = null!;
    internal static ConfigEntry<bool> EnableItemBlessedState = null!;
    internal static ConfigEntry<BlessedState> ItemBlessedState = null!;
    internal static ConfigEntry<bool> EnableExperienceMultiplier = null!;
    internal static ConfigEntry<int> ExperienceMultiplier = null!;
    internal static ConfigEntry<bool> EnableZeroWeight = null!;
    internal static ConfigEntry<bool> EnableSetTier = null!;
    internal static ConfigEntry<int> SetTier = null!;
    internal static ConfigEntry<bool> EnableNoBaitConsumption = null!;
    
    // QoL
    internal static ConfigEntry<bool> EnableAutoFish = null!;
    internal static ConfigEntry<bool> EnableTurboMode = null!;
    internal static ConfigEntry<int> TurboModeSpeedMultiplier = null!;
    internal static ConfigEntry<bool> EnableStaminaThreshold = null!;
    internal static ConfigEntry<int> StaminaThreshold = null!;
    internal static ConfigEntry<bool> EnableRippleEffect = null!;
    internal static ConfigEntry<bool> EnableAnimations = null!;
    internal static ConfigEntry<bool> EnableSounds = null!;
    internal static ConfigEntry<bool> EnableWinterFishing = null!;
    internal static ConfigEntry<bool> EnableAutoEat = null!;
    internal static ConfigEntry<int> AutoEatThreshold = null!;
    internal static ConfigEntry<bool> EnableAutoSleep = null!;
    internal static ConfigEntry<int> AutoSleepThreshold = null!;
    internal static ConfigEntry<bool> EnableAutoDump = null!;
    internal static ConfigEntry<AutoDumpTriggerMode> AutoDumpTrigger = null!;
    internal static ConfigEntry<int> AutoDumpThreshold = null!;
    internal static ConfigEntry<bool> EnableInstantBonitoFlakes = null!;
    internal static ConfigEntry<bool> EnableExcludeTierFishFromBonito = null!;
    internal static ConfigEntry<bool> EnableInstantWine = null!;
    internal static ConfigEntry<bool> EnableNoStaminaCost = null!;
    
    // Fish
    internal static ConfigEntry<string> _selectedFishIds = null!;
    
    internal static List<string> SelectedFishIds
    {
        get
        {
            string selectedFishIds = GetSelectedFishIdsValue();

            return selectedFishIds.Split(separator: ',')
                .Select(selector: id => id.Trim())
                .Where(predicate: id => string.IsNullOrEmpty(value: id) == false)
                .ToList();
        }
    }
    
    internal static void UpdateSelectedFishIds(List<string> selectedFishIdsList)
    {
        var selectedFishIdsEntry = _selectedFishIds;

        if (selectedFishIdsEntry == null)
        {
            return;
        }

        selectedFishIdsEntry.Value = string.Join(
            separator: ",",
            values: NormalizeFishIds(fishIds: selectedFishIdsList));
    }

    internal static void SetFishSelected(string fishId, bool isSelected)
    {
        HashSet<string> selectedFishIds = new HashSet<string>(collection: SelectedFishIds);

        if (isSelected)
        {
            selectedFishIds.Add(item: fishId);
        }
        else
        {
            selectedFishIds.Remove(item: fishId);
        }

        UpdateSelectedFishIds(selectedFishIdsList: selectedFishIds.ToList());
    }

    internal static List<SourceThing.Row> GetAvailableFishRows()
    {
        if (EClass.sources?.things?.map == null ||
            EClass.sources.categories?.map == null)
        {
            return new List<SourceThing.Row>();
        }

        List<SourceThing.Row> fishRows = new List<SourceThing.Row>();

        foreach (SourceThing.Row row in EClass.sources.things.map.Values)
        {
            if (row == null)
            {
                continue;
            }

            if (ExcludedFishIds.Contains(item: row.id))
            {
                continue;
            }

            if (int.TryParse(s: row.id, result: out int numericId) == false)
            {
                continue;
            }

            if (EClass.sources.categories.map.TryGetValue(key: row.category, value: out SourceCategory.Row categoryRow) == false)
            {
                continue;
            }

            if (categoryRow.IsChildOf(id: "fish") == false)
            {
                continue;
            }

            fishRows.Add(item: row);
        }

        return fishRows
            .OrderBy(keySelector: row => int.Parse(s: row.id))
            .ToList();
    }

    private static string GetSelectedFishIdsValue()
    {
        var selectedFishIdsEntry = _selectedFishIds;

        if (selectedFishIdsEntry != null &&
            string.IsNullOrWhiteSpace(value: selectedFishIdsEntry.Value) == false)
        {
            return selectedFishIdsEntry.Value;
        }

        if (TryBuildDefaultSelectedFishIds(selectedFishIds: out string dynamicSelectedFishIds) == true)
        {
            if (selectedFishIdsEntry != null)
            {
                selectedFishIdsEntry.Value = dynamicSelectedFishIds;
            }

            return dynamicSelectedFishIds;
        }

        return DefaultSelectedFishIds;
    }

    private static bool TryBuildDefaultSelectedFishIds(out string selectedFishIds)
    {
        selectedFishIds = string.Empty;
        List<string> fishIds = GetAvailableFishRows()
            .Select(selector: row => row.id)
            .ToList();

        if (fishIds.Count == 0)
        {
            return false;
        }

        selectedFishIds = string.Join(
            separator: ",",
            values: NormalizeFishIds(fishIds: fishIds));

        return true;
    }

    private static List<string> NormalizeFishIds(IEnumerable<string> fishIds)
    {
        return fishIds
            .Where(predicate: id => string.IsNullOrWhiteSpace(value: id) == false)
            .Select(selector: id => id.Trim())
            .Distinct()
            .OrderBy(keySelector: id => int.TryParse(s: id, result: out int numericId) ? numericId : int.MaxValue)
            .ToList();
    }
    
    public static string XmlPath { get; private set; } = string.Empty;
    public static string TranslationXlsxPath { get; private set; } = string.Empty;
    
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
        
        EnableInstantReadAncientBook = config.Bind(
            section: ModInfo.Name,
            key: "Enable Instant Read Ancient Book",
            defaultValue: false,
            description:
            "Enable or disable instantly reading caught ancient books.\n" +
            "Set to 'true' to read a caught ancient book immediately, or 'false' to leave it unread.\n" +
            "古書を自動で読む機能を有効または無効にします。\n" +
            "'true' に設定すると見つかった古書を自動で読みます。'false' にすると無視します。\n" +
            "启用或禁用自动阅读古书的功能。\n" +
            "设置为 'true' 会在发现古书时自动阅读，设置为 'false' 则忽略。");
        
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
        
        EnableSetTier = config.Bind(
            section: ModInfo.Name,
            key: "Enable Set Tier",
            defaultValue: false,
            description: "Enable or disable setting a custom tier level (★) for caught fish.\n" +
                         "Set to 'true' to override fish with a custom tier level (★).\n" +
                         "釣った魚にカスタムティア（★）を設定する機能を有効または無効にします。\n" +
                         "'true' に設定すると、魚のティア（★）を上書きします。\n" +
                         "启用或禁用为钓到的鱼设置自定义星级（★）。\n" +
                         "设置为 'true' 将覆盖鱼的原有星级（★）。"
        );
        
        SetTier = config.Bind(
            section: ModInfo.Name,
            key: "Set Tier",
            defaultValue: 0,
            description: "Set the tier level to apply to caught fish.\n" +
                         "0 = None, 1 = ★, 2 = ★★, 3 = ★★★, etc.\n" +
                         "釣った魚に適用するティアレベルを設定します。\n" +
                         "0 = なし、1 = ★、2 = ★★、3 = ★★★ など。\n" +
                         "设置钓到的鱼的星级。\n" +
                         "0 = 无，1 = ★，2 = ★★，3 = ★★★ 等。"
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
            defaultValue: false,
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
            defaultValue: 1,
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
            description: "Enable or disable automatic dumping of fishing items into containers.\n" +
                         "Set to 'true' to automatically place items into the nearest valid container when the selected auto dump trigger is reached, or 'false' to disable it.\n" +
                         "釣りで得たアイテムをコンテナに自動的に移動する機能を有効または無効にします。\n" +
                         "'true' に設定すると、選択した自動ダンプ条件に達したときにアイテムが自動でコンテナに移動され、'false' に設定すると無効になります。\n" +
                         "启用或禁用自动将钓鱼物品放入容器。\n" +
                         "设置为 'true' 时，会在达到所选自动倾倒条件时自动放入最近的有效容器；设置为 'false' 时禁用此功能。"
        );

        AutoDumpTrigger = config.Bind(
            section: ModInfo.Name,
            key: "Auto Dump Trigger Mode",
            defaultValue: Config.AutoDumpTriggerMode.FullOnly,
            description: "Select when automatic dumping should trigger.\n" +
                         "Available modes are Full Only, Inventory Usage %, and Carry Weight %.\n" +
                         "自動ダンプを開始する条件を選択します。\n" +
                         "利用可能なモードは、満杯のみ、所持数使用率%、重量使用率% です。\n" +
                         "选择自动倾倒的触发条件。\n" +
                         "可用模式包括仅满背包、背包占用率%、负重使用率%。"
        );

        AutoDumpThreshold = config.Bind(
            section: ModInfo.Name,
            key: "Auto Dump Threshold",
            defaultValue: 100,
            description: "Set the threshold percentage used by the Inventory Usage % and Carry Weight % auto dump modes.\n" +
                         "A value of 100 means the trigger only fires at full capacity for those modes.\n" +
                         "Inventory Usage % と Carry Weight % の自動ダンプモードで使用するしきい値の割合を設定します。\n" +
                         "100 に設定すると、これらのモードでも満杯時のみ発動します。\n" +
                         "设置 Inventory Usage % 和 Carry Weight % 自动倾倒模式使用的阈值百分比。\n" +
                         "设置为 100 时，这些模式也只会在满容量时触发。"
        );
        
        EnableInstantBonitoFlakes = config.Bind(
            section: ModInfo.Name,
            key: "Enable Instant Bonito Flakes",
            defaultValue: false,
            description: "Enable or disable instantly turning selected caught fish into bonito flakes.\n" +
                         "Set to 'true' to instantly convert selected caught fish into bonito flakes. If 'Enable Instant Wine' is also enabled, the bonito result can be converted onward into wine.\n" +
                         "選択した魚をかつおぶしに即座に変換する機能を有効または無効にします。\n" +
                         "'true' に設定すると、選択した魚を即座にかつおぶしに変換します。\n" +
                         "启用或禁用将选定鱼类即时转化为柴鱼片。\n" +
                         "设置为 'true' 可即时将选定鱼类转化为柴鱼片，设置为 'false' 则禁用此功能。"
        );
        
        EnableExcludeTierFishFromBonito = config.Bind(
            section: ModInfo.Name,
            key: "Enable Exclude Tier Fish From Bonito Flakes",
            defaultValue: false,
            description: "Enable or disable excluding star-tier (★) fish from the bonito flakes feature.\n" +
                         "Set to 'true' to skip converting rare fish marked with a star.\n" +
                         "星付き（★）のレア魚をかつおぶし変換機能の対象から除外するかどうかを設定します。\n" +
                         "'true' に設定すると、★付きの魚は変換されません。\n" +
                         "启用或禁用将带有星标（★）的稀有鱼类排除在柴鱼片转换功能之外。\n" +
                         "设置为 'true' 可跳过转换带星的稀有鱼类。"
        );
        
        EnableInstantWine = config.Bind(
            section: ModInfo.Name,
            key: "Enable Instant Wine",
            defaultValue: false,
            description: "Enable or disable instantly turning the current caught fish result into wine.\n" +
                         "Set to 'true' to instantly convert the current caught fish into wine. If 'Enable Instant Bonito Flakes' is also enabled, selected fish can chain from bonito flakes into wine.\n" +
                         "すべての魚アイテムを即座にワインに変換する機能を有効または無効にします。\n" +
                         "'true' に設定するとすべての魚アイテムを即座にワインに変換し、'false' に設定すると無効になります。\n" +
                         "启用或禁用将所有鱼类物品即时转化为葡萄酒。\n" +
                         "设置为 'true' 可即时将所有鱼类物品转化为葡萄酒，设置为 'false' 则禁用此功能。"
        );
        
        _selectedFishIds = config.Bind(
            section: ModInfo.Name,
            key: "SelectedFishIds",
            defaultValue: string.Empty,
            description: "Comma-separated list of fish IDs to use with the Instant Bonito Flakes feature.\n" +
                         "Relates to the following fish:\n" +
                         "Ancient Fish (92), Arowana (74), Bass (69), Bitterling (62), Black Bass (77), Blowfish (83), Bonito (81), Carp (65),\n" +
                         "Coelacanth (90), Deep Sea Fish (91), Eel (66), Flatfish (84), Goby (67), Goldfish (64), Mackerel (78),\n" +
                         "Moonfish (95), Muddler (68), Red Bream (79), Salmon (87), Sand Borer (88), Sardine (85), Scad (72),\n" +
                         "Sea Bream (86), Sea Urchin (70), Shark (93), Striped Jack (76), Sunfish (94), Sweetfish (75), Tadpole (63),\n" +
                         "Tilefish (71), Pintuna (73), Tuna (82), Turtle (80), Whale (89).\n" +
                         "即席かつお節機能で使用する魚のIDのカンマ区切りリストです。\n" +
                         "次の魚に関連付けられています:\n" +
                         "古代魚 (92)、アロワナ (74)、バス (69)、ビタリング (62)、ブラックバス (77)、フグ (83)、カツオ (81)、コイ (65)、\n" +
                         "シーラカンス (90)、深海魚 (91)、ウナギ (66)、ヒラメ (84)、ゴビ (67)、金魚 (64)、サバ (78)、\n" +
                         "ムーンフィッシュ (95)、マッドラー (68)、赤鯛 (79)、サーモン (87)、サンドボラー (88)、イワシ (85)、サバ (72)、\n" +
                         "鯛 (86)、ウニ (70)、サメ (93)、シマアジ (76)、マンボウ (94)、アユ (75)、オタマジャクシ (63)、\n" +
                         "アマダイ (71)、マグロ 1 (73)、マグロ 2 (82)、カメ (80)、クジラ (89)。\n" +
                         "用于即时柴鱼片功能的鱼类ID的逗号分隔列表。\n" +
                         "与以下鱼类相关:\n" +
                         "远古鱼 (92)、龙鱼 (74)、鲈鱼 (69)、苦鱼 (62)、黑鲈 (77)、河豚 (83)、鲣鱼 (81)、鲤鱼 (65)、\n" +
                         "腔棘鱼 (90)、深海鱼 (91)、鳗鱼 (66)、比目鱼 (84)、虾虎鱼 (67)、金鱼 (64)、鲭鱼 (78)、\n" +
                         "月鱼 (95)、泥鳅 (68)、红鲷鱼 (79)、三文鱼 (87)、沙钻鱼 (88)、沙丁鱼 (85)、竹荚鱼 (72)、\n" +
                         "真鲷 (86)、海胆 (70)、鲨鱼 (93)、条纹鰺 (76)、翻车鱼 (94)、香鱼 (75)、蝌蚪 (63)、\n" +
                         "瓦鱼 (71)、金枪鱼 1 (73)、金枪鱼 2 (82)、海龟 (80)、鲸鱼 (89)。"
        );
        
        EnableNoStaminaCost = config.Bind(
            section: ModInfo.Name,
            key: "Enable No Stamina Cost",
            defaultValue: false,
            description:
            "Enable or disable no stamina cost while fishing.\n" +
            "Set to 'true' to prevent stamina being consumed during fishing, or 'false' to use normal stamina costs.\n" +
            "釣り中のスタミナ消費なしを有効または無効にします。\n" +
            "「true」に設定すると釣り中にスタミナを消費しません。「false」に設定すると通常どおりスタミナを消費します。\n" +
            "启用或禁用钓鱼时不消耗体力。\n" +
            "设置为 'true' 时钓鱼不消耗体力，设置为 'false' 时按正常规则消耗体力。"
        );
        
        EnableNoBaitConsumption = config.Bind(
            section: ModInfo.Name,
            key: "Enable No Bait Consumption",
            defaultValue: false,
            description:
            "Enable or disable no bait consumption while fishing.\n" +
            "Set to 'true' to prevent bait from being consumed during fishing, or 'false' to use bait normally.\n" +
            "釣り中の餌消費なしを有効または無効にします。\n" +
            "「true」に設定すると釣り中に餌を消費しません。「false」に設定すると通常どおり餌を消費します。\n" +
            "启用或禁用钓鱼时不消耗鱼饵。\n" +
            "设置为 'true' 时钓鱼不消耗鱼饵，设置为 'false' 时按正常规则消耗鱼饵。"
        );
    }

    public static void InitializeXmlPath(string xmlPath)
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
    
    public static void InitializeTranslationXlsxPath(string xlsxPath)
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
