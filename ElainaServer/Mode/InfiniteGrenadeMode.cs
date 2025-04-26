using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;

namespace ElainaServer;

public class InfiniteGrenadeMode(BaseMode plugin) : BaseMode(plugin)
{
	public override string ModeName => "Infinite Grenade";
	public override string ModeDescription => "All players have infinite grenades!";

	public InfiniteGrenadeMode() : this(null!)
	{
	}

	private static void GiveAllPlayersGrenades()
	{
		var Allplayers = Utilities.GetPlayers();
		bool BombHasGiven = false;
		foreach (var player in Allplayers)
		{
			if (!player.IsValid) continue;
			player.RemoveWeapons();
			player.GiveNamedItem(CsItem.HE);
			player.GiveNamedItem(CsItem.Knife);
			player.GiveNamedItem(CsItem.Kevlar);
			player.GiveNamedItem(CsItem.KevlarHelmet);
			if (player.Team == CsTeam.Terrorist && !BombHasGiven)
			{
				player.GiveNamedItem(CsItem.C4);
				BombHasGiven = true;
			}
		}
	}

	private readonly BasePlugin.GameEventHandler<EventRoundStart> EventRoundStartHandler = (@event, info) =>
	{
		GiveAllPlayersGrenades();

		return HookResult.Stop;
	};


	private int SvBuyStatusOverride = 0;
	private bool MpAutokick = false;
	private int SvInfiniteAmmo = 0;
	private int AmmoGrenadeLimitTotal = 0;
	private int AmmoGrenadeLimitDefault = 0;
	private int MpWeaponsAllowRifles = 0;
	private int MpWeaponsAllowPistols = 0;
	private int MpWeaponsAllowSmgs = 0;
	private int MpWeaponsAllowHeavy = 0;

	public override void OnModeLoad(ElainaServer plugin)
	{
		plugin.RegisterEventHandler<EventRoundStart>(EventRoundStartHandler);

		SvBuyStatusOverride = ConVar.Find("sv_buy_status_override")!.GetPrimitiveValue<Int32>();
		MpAutokick = ConVar.Find("mp_autokick")!.GetPrimitiveValue<bool>();
		SvInfiniteAmmo = ConVar.Find("sv_infinite_ammo")!.GetPrimitiveValue<Int32>();
		AmmoGrenadeLimitTotal = ConVar.Find("ammo_grenade_limit_total")!.GetPrimitiveValue<Int32>();
		AmmoGrenadeLimitDefault = ConVar.Find("ammo_grenade_limit_default")!.GetPrimitiveValue<Int32>();
		MpWeaponsAllowRifles = ConVar.Find("mp_weapons_allow_rifles")!.GetPrimitiveValue<Int32>();
		MpWeaponsAllowPistols = ConVar.Find("mp_weapons_allow_pistols")!.GetPrimitiveValue<Int32>();
		MpWeaponsAllowSmgs = ConVar.Find("mp_weapons_allow_smgs")!.GetPrimitiveValue<Int32>();
		MpWeaponsAllowHeavy = ConVar.Find("mp_weapons_allow_heavy")!.GetPrimitiveValue<Int32>();

		// 3 is disabled buy menu for anyone
		ConVar.Find("sv_buy_status_override")!.SetValue(3);
		ConVar.Find("mp_autokick")!.SetValue(false);
		ConVar.Find("sv_infinite_ammo")!.SetValue(1);
		ConVar.Find("ammo_grenade_limit_total")!.SetValue(10);
		ConVar.Find("ammo_grenade_limit_default")!.SetValue(2);
		ConVar.Find("mp_weapons_allow_rifles")!.SetValue(0);
		ConVar.Find("mp_weapons_allow_pistols")!.SetValue(0);
		ConVar.Find("mp_weapons_allow_smgs")!.SetValue(0);
		ConVar.Find("mp_weapons_allow_heavy")!.SetValue(0);


		GiveAllPlayersGrenades();
	}

	public override void OnModeUnload(ElainaServer plugin)
	{
		plugin.DeregisterEventHandler<EventRoundStart>(EventRoundStartHandler);

		ConVar.Find("sv_buy_status_override")!.SetValue(SvBuyStatusOverride);
		ConVar.Find("mp_autokick")!.SetValue(MpAutokick);
		ConVar.Find("sv_infinite_ammo")!.SetValue(SvInfiniteAmmo);
		ConVar.Find("ammo_grenade_limit_total")!.SetValue(AmmoGrenadeLimitTotal);
		ConVar.Find("ammo_grenade_limit_default")!.SetValue(AmmoGrenadeLimitDefault);
		ConVar.Find("mp_weapons_allow_rifles")!.SetValue(MpWeaponsAllowRifles);
		ConVar.Find("mp_weapons_allow_pistols")!.SetValue(MpWeaponsAllowPistols);
		ConVar.Find("mp_weapons_allow_smgs")!.SetValue(MpWeaponsAllowSmgs);
		ConVar.Find("mp_weapons_allow_heavy")!.SetValue(MpWeaponsAllowHeavy);
	}
}
