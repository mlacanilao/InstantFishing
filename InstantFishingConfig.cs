using BepInEx.Configuration;

namespace InstantFishing
{
    internal static class InstantFishingConfig
    {
        internal static ConfigEntry<bool> EnableInstantFishing;
        internal static ConfigEntry<int> HitValue;
        internal static ConfigEntry<bool> EnableTurboMode;
        internal static ConfigEntry<int> TurboSpeed;
        internal static ConfigEntry<bool> EnableStaminaCheck;
        internal static ConfigEntry<bool> ShouldCancel;
        internal static ConfigEntry<int> StaminaThreshold;

        /// <summary>
        /// Loads all configurations for the InstantFishing mod.
        /// </summary>
        /// <param name="config">The BepInEx ConfigFile to bind configurations to.</param>
        internal static void LoadConfig(ConfigFile config)
        {
            EnableInstantFishing = config.Bind(
                section: ModInfo.Name,
                key: "Enable Instant Fishing",
                defaultValue: true,
                description: "Enable or disable the Instant Fishing mod.\n" +
                             "Set to 'true' to activate the mod, or 'false' to keep the game unchanged.\n" +
                             "インスタントフィッシングMODを有効または無効にします。\n" +
                             "'true' に設定するとMODが有効になり、'false' に設定するとゲームのデフォルトのままになります。");

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
                             "If the owner's stamina drops below this value, the fishing action will cancel.\n" +
                             "釣りを続けるために必要な最小スタミナを設定します。\n" +
                             "所有者のスタミナがこの値を下回ると、釣りアクションがキャンセルされます。");
        }
    }
}
