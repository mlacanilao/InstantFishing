using BepInEx.Configuration;

namespace InstantFishing
{
    internal static class InstantFishingConfig
    {
        internal static ConfigEntry<bool> EnableInstantFishingMod;
        internal static ConfigEntry<bool> EnableHitValue;
        internal static ConfigEntry<int> HitValue;
        internal static ConfigEntry<bool> EnableTurboMode;
        internal static ConfigEntry<int> TurboSpeed;
        internal static ConfigEntry<bool> EnableStaminaCheck;
        internal static ConfigEntry<bool> ShouldCancel;
        internal static ConfigEntry<int> StaminaThreshold;
        internal static ConfigEntry<bool> EnableRipple;
        internal static ConfigEntry<bool> EnableItemMultiplier;
        internal static ConfigEntry<int> ItemMultiplier;
        internal static ConfigEntry<bool> EnableItemBlessedState;
        internal static ConfigEntry<bool> IsBlessed;

        internal static void LoadConfig(ConfigFile config)
        {
            EnableInstantFishingMod = config.Bind(
                section: ModInfo.Name,
                key: "Enable Instant Fishing Mod",
                defaultValue: true,
                description: "Enable or disable the Instant Fishing mod.\n" +
                             "Set to 'true' to activate the mod, or 'false' to keep the game unchanged.\n" +
                             "インスタントフィッシングMODを有効または無効にします。\n" +
                             "'true' に設定するとMODが有効になり、'false' に設定するとゲームのデフォルトのままになります。");
            
            EnableHitValue = config.Bind(
                section: ModInfo.Name,
                key: "Enable Hit Value",
                defaultValue: true,
                description: "Enable or disable the custom hit value for instant fishing.\n" +
                             "Set to 'true' to use a custom hit value, or 'false' to disable it.\n" +
                             "カスタムのヒット値を有効または無効にします。\n" +
                             "'true' に設定するとカスタムヒット値が使用され、'false' に設定すると無効になります。");

            HitValue = config.Bind(
                section: ModInfo.Name,
                key: "Hit Value",
                defaultValue: 100,
                description: "Set the progress value for catching fish instantly.\n" +
                             "Higher values make it easier to catch fish instantly.\n" +
                             "魚を即座に捕まえるための進行値を設定します。\n" +
                             "値が高いほど、魚を簡単に捕まえられます。");

            EnableTurboMode = config.Bind(
                section: ModInfo.Name,
                key: "Enable Turbo Mode",
                defaultValue: true,
                description: "Enable or disable Turbo Mode for faster fishing.\n" +
                             "Set to 'true' to activate Turbo Mode, or 'false' to disable it.\n" +
                             "釣りのターボモードを有効または無効にします。\n" +
                             "'true' に設定するとターボモードが有効になり、'false' に設定すると無効になります。");

            TurboSpeed = config.Bind(
                section: ModInfo.Name,
                key: "Turbo Speed",
                defaultValue: 3,
                description: "Set the speed multiplier for Turbo Mode during fishing.\n" +
                             "Must be an integer value (e.g., 2 for double speed).\n" +
                             "釣りのターボモード中のスピード倍率を設定します。\n" +
                             "整数値である必要があります（例: 倍速の場合は2）。");

            EnableStaminaCheck = config.Bind(
                section: ModInfo.Name,
                key: "Enable Stamina Check",
                defaultValue: true,
                description: "Enable or disable the stamina check during fishing.\n" +
                             "Set to 'true' to activate the stamina check, or 'false' to skip it.\n" +
                             "釣り中のスタミナチェックを有効または無効にします。\n" +
                             "'true' に設定するとスタミナチェックが有効になり、'false' に設定すると無効になります。");

            ShouldCancel = config.Bind(
                section: ModInfo.Name,
                key: "Should Cancel",
                defaultValue: true,
                description: "Control whether the fishing action should cancel on specific conditions.\n" +
                             "Set to 'true' to enable cancellation checks, or 'false' to disable them.\n" +
                             "特定の条件で釣りアクションをキャンセルするかどうかを設定します。\n" +
                             "'true' に設定するとキャンセルチェックが有効になり、'false' に設定すると無効になります。");

            StaminaThreshold = config.Bind(
                section: ModInfo.Name,
                key: "Stamina Threshold",
                defaultValue: 1,
                description: "Set the minimum stamina required to continue fishing.\n" +
                             "If the player's stamina drops below this value, the fishing action will cancel.\n" +
                             "釣りを続けるために必要な最小スタミナを設定します。\n" +
                             "所有者のスタミナがこの値を下回ると、釣りアクションがキャンセルされます。");
            
            EnableRipple = config.Bind(
                section: ModInfo.Name,
                key: "Enable Ripple Effect",
                defaultValue: true,
                description: "Enable or disable the ripple effect during fishing.\n" +
                             "Set to 'true' to play ripple animations and sound effects when fishing, or 'false' to disable them.\n" +
                             "釣り中のリップルエフェクトを有効または無効にします。\n" +
                             "'true' に設定すると釣り中にリップルアニメーションと効果音が再生され、'false' に設定すると無効になります。");
            
            EnableItemMultiplier = config.Bind(
                section: ModInfo.Name,
                key: "Enable Item Multiplier",
                defaultValue: true,
                description: "Enable or disable the item multiplier during fishing.\n" +
                             "Set to 'true' to activate item multiplier logic, or 'false' to disable it.\n" +
                             "釣り中のアイテム倍率を有効または無効にします。\n" +
                             "'true' に設定するとアイテム倍率ロジックが有効になり、'false' に設定すると無効になります。");
            
            ItemMultiplier = config.Bind(
                section: ModInfo.Name,
                key: "Item Multiplier",
                defaultValue: 1,
                description: "Set the multiplier for the number of items caught during fishing.\n" +
                             "Higher values increase the number of items caught.\n" +
                             "Must be an integer value (e.g., 2 for double items).\n" +
                             "釣り中に捕まえるアイテム数の倍率を設定します。\n" +
                             "値が高いほど、捕まえるアイテム数が増加します。\n" +
                             "整数値である必要があります（例: 2 はアイテムが2倍になります）。");
            
            EnableItemBlessedState = config.Bind(
                section: ModInfo.Name,
                key: "Enable Item Blessed State",
                defaultValue: true,
                description: "Enable or disable setting a custom blessed state for items.\n" +
                             "Set to 'true' to allow items to have a custom blessed state, or 'false' to disable it.\n" +
                             "アイテムにカスタム祝福状態を設定するかどうかを有効または無効にします。\n" +
                             "'true' に設定するとカスタム祝福状態が許可され、'false' に設定すると無効になります。");

            IsBlessed = config.Bind(
                section: ModInfo.Name,
                key: "Is Blessed",
                defaultValue: false,
                description: "Determine whether items should be blessed or normal.\n" +
                             "Set to 'true' for blessed items, or 'false' for normal items.\n" +
                             "アイテムを祝福状態にするか通常状態にするかを決定します。\n" +
                             "'true' に設定するとアイテムが祝福状態になり、'false' に設定すると通常状態になります。");
        }
    }
}
