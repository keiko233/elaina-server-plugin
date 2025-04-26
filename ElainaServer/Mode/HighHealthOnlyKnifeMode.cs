using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core.Translations;

namespace ElainaServer;

public class HighHealthOnlyKnifeMode(BaseMode plugin) : BaseMode(plugin)
{
	public override string ModeName => "High Health Only Knife Mode";
	public override string ModeDescription => "One player from each team gets 10000 HP, everyone only knife";
	public HighHealthOnlyKnifeMode() : this(null!)
	{
	}

	private const int HEALTH = 10000;
	private int PrimitiveSvBuyStatusOverride = 0;
	private readonly Random random = new();

	public override void OnModeLoad(ElainaServer plugin)
	{
		PrimitiveSvBuyStatusOverride = ConVar.Find("sv_buy_status_override")!.GetPrimitiveValue<Int32>();
		// 3 is disabled buy menu for anyone
		ConVar.Find("sv_buy_status_override")!.SetValue(3);

		List<CCSPlayerController> Allplayers = Utilities.GetPlayers();

		var ctPlayers = new List<CCSPlayerController>();
		var tPlayers = new List<CCSPlayerController>();

		foreach (var player in Allplayers)
		{
			if (player == null || !player.IsValid) continue;

			if (player.Team == CsTeam.CounterTerrorist) ctPlayers.Add(player);
			else if (player.Team == CsTeam.Terrorist) tPlayers.Add(player);
		}

		CCSPlayerController? chosenCT = ctPlayers.Count > 0 ? ctPlayers[random.Next(ctPlayers.Count)] : null;
		CCSPlayerController? chosenT = tPlayers.Count > 0 ? tPlayers[random.Next(tPlayers.Count)] : null;

		foreach (var player in Allplayers)
		{
			var origin = player!.OriginalControllerOfCurrentPawn.Get()!;
			if (origin is null) continue;
			CCSPlayerPawn? pawn = origin.PlayerPawn.Get();
			if (pawn is null) continue;
			if (!pawn!.IsValid) continue;

			// TODO: remove weapons without current knife
			player.RemoveWeapons();

			player.GiveNamedItem(CsItem.Knife);
			player.GiveNamedItem(CsItem.Kevlar);
			player.GiveNamedItem(CsItem.KevlarHelmet);

			if (player == chosenCT || player == chosenT)
			{
				pawn!.MaxHealth = HEALTH;
				pawn!.Health = HEALTH;

				string teamName = player.Team == CsTeam.CounterTerrorist ? "CT" : "T";
				Server.PrintToChatAll(StringExtensions.ReplaceColorTags($"{{GREEN}}[{plugin.ModuleName}] {{RED}}{player.PlayerName}{{WHITE}} is selected as high health player in {{YELLOW}}{teamName}{{WHITE}} team!"));
			}
			else
			{
				pawn!.MaxHealth = 100;
				pawn!.Health = 100;
			}

			Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iMaxHealth");
			Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
		}
	}

	public override void OnModeUnload(ElainaServer plugin)
	{
		ConVar.Find("sv_buy_status_override")!.SetValue(PrimitiveSvBuyStatusOverride);

		List<CCSPlayerController> Allplayers = Utilities.GetPlayers();

		foreach (var player in Allplayers)
		{
			var origin = player!.OriginalControllerOfCurrentPawn.Get()!;
			if (origin is null) continue;
			CCSPlayerPawn? pawn = origin.PlayerPawn.Get();
			if (pawn is null) continue;
			if (!pawn!.IsValid) continue;

			pawn!.MaxHealth = 100;
			pawn!.Health = 100;

			Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iMaxHealth");
			Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
		}
	}
}
