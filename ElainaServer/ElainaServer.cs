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

	public override void Load(bool hotReload)
	{
		Logger.LogInformation("ElainaServer Plugin Loaded!");
	}

	private BaseMode? PendingMode = null;
	private bool IsRandomModeEnabled = false;

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
	];

	[GameEventHandler]
	public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
	{
		if (!IsRandomModeEnabled) return HookResult.Continue;

		// Unload any previous mode
		if (PendingMode != null)
		{
			PendingMode.OnModeUnload(this);
			PendingMode = null;
		}

		// Get random mode, avoiding the previously selected one
		Random random = new();
		int randomIndex;

		do
		{
			randomIndex = random.Next(0, ModeLists.Count);
		} while
			(ModeLists.Count > 1 && PendingMode != null && randomIndex == ModeLists.IndexOf(PendingMode));

		// Activate the new mode
		PendingMode = ModeLists[randomIndex];
		PendingMode.OnModeLoad(this);
		PendingMode.PrintModeDescriptionToChatAll(this);

		return HookResult.Continue;
	}


	[GameEventHandler]
	public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
	{
		if (@event.Userid == null) { return HookResult.Stop; }

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

			commandInfo.ReplyToCommand($"{i} {currentMode.ModeName} {currentMode.ModeDescription}");
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

		commandInfo.ReplyToCommand($"Loaded mode: {PendingMode.ModeName}");

		PendingMode.OnModeLoad(this);
		PendingMode.PrintModeDescriptionToChatAll(this);
	}

	[ConsoleCommand("css_unload_mode", "Unload random mode")]
	[CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
	[RequiresPermissions("@css/root")]
	public void OnUnloadRandomModeCommand(CCSPlayerController? player, CommandInfo commandInfo)
	{
		if (PendingMode == null) { return; }

		PendingMode.OnModeUnload(this);

		commandInfo.ReplyToCommand($"Unloaded mode: {PendingMode?.ModeName}");
		Server.PrintToChatAll(StringExtensions.ReplaceColorTags($"{{GREEN}}[{ModuleName}] {{WHITE}}Unloaded mode: {PendingMode?.ModeName}"));

		PendingMode = null;
	}

	[ConsoleCommand("css_enable_random_mode", "Enable random mode")]
	[CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
	[RequiresPermissions("@css/root")]
	public void OnEnableRandomModeCommand(CCSPlayerController? player, CommandInfo commandInfo)
	{
		IsRandomModeEnabled = true;
	}

	[ConsoleCommand("css_disable_random_mode", "Disable random mode")]
	[CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
	[RequiresPermissions("@css/root")]
	public void OnDisableRandomModeCommand(CCSPlayerController? player, CommandInfo commandInfo)
	{
		IsRandomModeEnabled = false;
	}
}
