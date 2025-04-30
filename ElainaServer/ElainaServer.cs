using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Logging;

namespace ElainaServer;

public class ElainaServer : BasePlugin
{

	public override string ModuleName => "Elaina Server";
	public override string ModuleVersion => "1.0.0";
	public override string ModuleAuthor => "keiko233";
	public override string ModuleDescription => "Elaina Server Plugin";

	private readonly List<BaseMode> ModeLists = [
		new HighHealthOnlyKnifeMode(),
		new SwitchWeaponWhenHitMode(),
		new HeadshotOnlyMode(),
		new DeathRemoveC4Mode(),
		new PlantC4AnywhereMode(),
		new TaserWithBhopMode(),
		new InfiniteGrenadeMode(),
		new SwapPositionsOnHitMode(),
		new ChangeWeaponOnShootMode(),
		new DropWeaponOnShootMode(),
	];

	public override void Load(bool hotReload)
	{
		Logger.LogInformation("ElainaServer Plugin Loaded!");
	}

	private BaseMode? PendingMode = null;
	private BaseMode? NextMode = null;

	private bool IsRandomModeEnabled = false;

	[GameEventHandler]
	public HookResult OnRoundStart(EventGameEnd @event, GameEventInfo info)
	{
		if (PendingMode != null)
		{
			PendingMode.OnModeUnload(this);
			PendingMode = null;
		}

		return HookResult.Continue;
	}

	[GameEventHandler]
	public HookResult OnRoundPrestart(EventRoundPrestart @event, GameEventInfo info)
	{
		if (!IsRandomModeEnabled) return HookResult.Continue;

		// Set the next round mode to the current mode
		if (NextMode != null)
		{
			PendingMode = NextMode;
			NextMode = null;
		}
		else
		{
			// Randomly select a mode when first running or NextMode is null
			Random random = new();
			int randomIndex = random.Next(0, ModeLists.Count);
			PendingMode = ModeLists[randomIndex];
		}

		// Load the pending mode
		if (PendingMode != null)
		{
			PendingMode.OnModeLoad(this);
			PendingMode.PrintModeDescriptionToChatAll(this);
		}

		// Randomly select a new mode for the next round, avoiding the same mode as the current one
		if (ModeLists.Count > 1)
		{
			Random random = new();
			int randomIndex;

			do
			{
				randomIndex = random.Next(0, ModeLists.Count);
			} while (PendingMode != null && randomIndex == ModeLists.IndexOf(PendingMode));

			NextMode = ModeLists[randomIndex];
			var name = Localizer[$"mode.{NextMode.ModeLocalizerId}.name"];
			Server.PrintToConsole($"Next mode: {name}");
		}

		return HookResult.Continue;
	}

	[GameEventHandler]
	public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
	{
		if (PendingMode != null)
		{
			PendingMode.OnModeUnload(this);
			PendingMode = null;
		}

		return HookResult.Stop;
	}

	[GameEventHandler]
	public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
	{
		if (@event.Userid == null) { return HookResult.Stop; }

		// ref: https://github.com/roflmuffin/CounterStrikeSharp/issues/846
		// var steamId = (CounterStrikeSharp.API.Modules.Entities.SteamID)@event.Userid.SteamID;
		// PlayerLanguageManager playerLanguage = new();
		// playerLanguage.SetLanguage(steamId, new System.Globalization.CultureInfo("zh"));

		var playerName = @event.Userid.PlayerName;
		Server.PrintToChatAll(StringExtensions.ReplaceColorTags($"{{GREEN}}[{ModuleName}] {{WHITE}}Hello {playerName}, Welcome to {ModuleName}!"));

		return HookResult.Stop;
	}

	[GameEventHandler]
	public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
	{
		if (@event.Userid == null) { return HookResult.Stop; }

		var playerName = @event.Userid.PlayerName;

		Server.PrintToChatAll(StringExtensions.ReplaceColorTags($"{{GREEN}}[{ModuleName}] {{WHITE}}{playerName} left the server!"));

		return HookResult.Stop;
	}

	[ConsoleCommand("css_list_all_mode", "List all random mode")]
	[CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
	[RequiresPermissions("@css/root")]
	public void OnListRandomModeCommand(CCSPlayerController? player, CommandInfo commandInfo)
	{
		for (int i = 0; i < ModeLists.Count; i++)
		{
			var currentMode = ModeLists[i];
			var name = Localizer[$"mode.{currentMode.ModeLocalizerId}.name"];
			var description = Localizer[$"mode.{currentMode.ModeLocalizerId}.description"];

			commandInfo.ReplyToCommand($"[{i}] {name}, {description}");
		}
	}

	[ConsoleCommand("css_load_mode", "Load random mode by index")]
	[CommandHelper(minArgs: 1, usage: "<index>", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
	[RequiresPermissions("@css/root")]
	public void OnLoadRandomModeCommand(CCSPlayerController? player, CommandInfo commandInfo)
	{
		_ = int.TryParse(commandInfo.GetArg(1), out int index);

		if (index < 0 || index >= ModeLists.Count) { return; }
		PendingMode = ModeLists[index];
		if (PendingMode == null) { return; }

		var name = Localizer[$"mode.{PendingMode.ModeLocalizerId}.name"];
		commandInfo.ReplyToCommand($"Loaded mode: {name}");

		PendingMode.OnModeLoad(this);
		PendingMode.PrintModeDescriptionToChatAll(this);
	}

	[ConsoleCommand("css_next_mode", "Load next random mode")]
	[CommandHelper(minArgs: 1, usage: "<index>", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
	[RequiresPermissions("@css/root")]
	public void OnNextRandomModeCommand(CCSPlayerController? player, CommandInfo commandInfo)
	{
		_ = int.TryParse(commandInfo.GetArg(1), out int index);

		if (index < 0 || index >= ModeLists.Count) { return; }
		NextMode = ModeLists[index];
		if (NextMode == null) { return; }

		var name = Localizer[$"mode.{NextMode.ModeLocalizerId}.name"];
		commandInfo.ReplyToCommand($"Next mode: {name}");
	}

	[ConsoleCommand("css_unload_mode", "Unload random mode")]
	[CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
	[RequiresPermissions("@css/root")]
	public void OnUnloadRandomModeCommand(CCSPlayerController? player, CommandInfo commandInfo)
	{
		if (PendingMode == null) { return; }

		PendingMode.OnModeUnload(this);

		var name = Localizer[$"mode.{PendingMode.ModeLocalizerId}.name"];
		commandInfo.ReplyToCommand($"Unloaded mode: {name}");
		Server.PrintToChatAll(StringExtensions.ReplaceColorTags($"{{GREEN}}[{ModuleName}] {{WHITE}}Unloaded mode: {name}"));

		PendingMode = null;
	}

	[ConsoleCommand("css_enable_random_mode", "Enable random mode")]
	[CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
	[RequiresPermissions("@css/root")]
	public void OnEnableRandomModeCommand(CCSPlayerController? player, CommandInfo commandInfo)
	{
		IsRandomModeEnabled = true;
		commandInfo.ReplyToCommand($"Random mode: {IsRandomModeEnabled}");
	}

	[ConsoleCommand("css_disable_random_mode", "Disable random mode")]
	[CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
	[RequiresPermissions("@css/root")]
	public void OnDisableRandomModeCommand(CCSPlayerController? player, CommandInfo commandInfo)
	{
		IsRandomModeEnabled = false;
		commandInfo.ReplyToCommand($"Random mode: {IsRandomModeEnabled}");
	}
}
