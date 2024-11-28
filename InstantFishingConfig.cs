using BepInEx.Configuration;

namespace InstantFishing
{
    internal static class InstantFishingConfig
    {
        internal static ConfigEntry<bool> EnableInstantFishing;
        internal static ConfigEntry<int> HitValue;
        internal static ConfigEntry<bool> EnableTurboMode;
        internal static ConfigEntry<int> TurboSpeed;

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
                             "インスタントフィッシングMODを有効または無効にします。");

            HitValue = config.Bind(
                section: ModInfo.Name,
                key: "Hit Value",
                defaultValue: 100,
                description: "The value to set for fish progress.\n" +
                             "魚の進行度に設定する値です。");

            EnableTurboMode = config.Bind(
                section: ModInfo.Name,
                key: "Enable Turbo Mode",
                defaultValue: true,
                description: "Enable or disable Turbo Mode for fishing.\n" +
                             "釣りのターボモードを有効または無効にします。");

            TurboSpeed = config.Bind(
                section: ModInfo.Name,
                key: "Turbo Speed",
                defaultValue: 3,
                description: "Set the speed multiplier for Turbo Mode. Must be an integer value (e.g., 2 for double speed).\n" +
                             "ターボモードのスピード倍率を設定します。整数値である必要があります（例: 倍速の場合は2）。");
        }
    }
}