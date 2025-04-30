using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;

namespace ElainaServer;

public abstract class BaseMode
{
	public abstract string ModeLocalizerId { get; }
	public BaseMode(ElainaServer plugin)
	{
		RegisterCommand(plugin);
	}

	public BaseMode(BaseMode plugin) { }

	public abstract void OnModeLoad(ElainaServer plugin);

	public abstract void OnModeUnload(ElainaServer plugin);

	public virtual void RegisterCommand(ElainaServer plugin) { }

	public virtual void PrintModeDescriptionToChatAll(ElainaServer plugin)
	{
		var name = plugin.Localizer[$"mode.{ModeLocalizerId}.name"];
		var description = plugin.Localizer[$"mode.{ModeLocalizerId}.description"];
		var prefix = $"{ChatColors.Green}[{plugin.ModuleName}] {ChatColors.Red}[{name}]";

		Server.PrintToChatAll(StringExtensions.ReplaceColorTags($"{prefix}{{WHITE}}: {description}"));
	}
}
