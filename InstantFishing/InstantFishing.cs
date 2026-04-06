using System;
using System.Runtime.CompilerServices;
using BepInEx;
using HarmonyLib;
using InstantFishing.Config;

namespace InstantFishing;

internal static class ModInfo
{
    internal const string Guid = "omegaplatinum.elin.instantfishing";
    internal const string Name = "Instant Fishing";
    internal const string Version = "3.0.0";
    internal const string ModOptionsGuid = "evilmask.elinplugins.modoptions";
}

[BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
internal class InstantFishing : BaseUnityPlugin
{
    private static int? lastLoggedInvalidTurboModeSpeedMultiplier;

    internal static InstantFishing? Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        InstantFishingConfig.LoadConfig(config: Config);
        LogInvalidTurboModeSpeedMultiplierIfNeeded();
        Harmony.CreateAndPatchAll(type: typeof(Patcher), harmonyInstanceId: ModInfo.Guid);

        if (HasModOptionsPlugin() == false)
        {
            return;
        }

        try
        {
            UI.UIController.RegisterUI();
        }
        catch (Exception ex)
        {
            LogError(message: $"An error occurred during UI registration: {ex}");
        }
    }

    internal static void LogDebug(object message, [CallerMemberName] string caller = "")
    {
        Instance?.Logger.LogDebug(data: $"[{caller}] {message}");
    }

    internal static void LogInfo(object message)
    {
        Instance?.Logger.LogInfo(data: message);
    }

    internal static void LogError(object message)
    {
        Instance?.Logger.LogError(data: message);
    }

    internal static int GetEffectiveTurboModeSpeedMultiplier()
    {
        int configuredMultiplier = InstantFishingConfig.TurboModeSpeedMultiplier.Value;
        if (configuredMultiplier > 0)
        {
            lastLoggedInvalidTurboModeSpeedMultiplier = null;
            return configuredMultiplier;
        }

        LogInvalidTurboModeSpeedMultiplierIfNeeded();
        return 1;
    }

    internal static void DisableTurboAndFlushRoundTimers()
    {
        if (EClass.core?.IsGameStarted != true)
        {
            return;
        }

        ActionMode.Adv.SetTurbo(mtp: -1);
        EClass._map?.charas?.ForEach(action: chara => chara.roundTimer = 0f);
    }

    private static void LogInvalidTurboModeSpeedMultiplierIfNeeded()
    {
        int configuredMultiplier = InstantFishingConfig.TurboModeSpeedMultiplier.Value;
        if (configuredMultiplier > 0)
        {
            lastLoggedInvalidTurboModeSpeedMultiplier = null;
            return;
        }

        if (lastLoggedInvalidTurboModeSpeedMultiplier == configuredMultiplier)
        {
            return;
        }

        lastLoggedInvalidTurboModeSpeedMultiplier = configuredMultiplier;
        LogInfo(
            $"Turbo Mode Speed Multiplier is {configuredMultiplier}. " +
            "Using 1 instead because the multiplier must be greater than 0.");
    }

    private static bool HasModOptionsPlugin()
    {
        try
        {
            foreach (var obj in ModManager.ListPluginObject)
            {
                if (obj is not BaseUnityPlugin plugin)
                {
                    continue;
                }

                if (plugin.Info.Metadata.GUID == ModInfo.ModOptionsGuid)
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            LogError(message: $"Error while checking for Mod Options: {ex}");
            return false;
        }
    }
}
