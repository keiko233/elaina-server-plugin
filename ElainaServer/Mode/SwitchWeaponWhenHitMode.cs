using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace ElainaServer;

public class SwitchWeaponWhenHitMode(BaseMode plugin) : BaseMode(plugin)
{
	public override string ModeName => "Use Your Opponent's Weapon When Hit Mode";
	public override string ModeDescription => "When you hit your opponent, exchange weapons with him";

	public SwitchWeaponWhenHitMode() : this(null!)
	{
	}

	private static HookResult GiveRandomWeaponForEveryone()
	{

		List<CCSPlayerController> Allplayers = Utilities.GetPlayers();

		foreach (var player in Allplayers)
		{
			if (player == null || !player.IsValid) continue;

			var originPlayer = player!.OriginalControllerOfCurrentPawn.Get()!;
			if (originPlayer is null) continue;
			CCSPlayerPawn? pawn = originPlayer.PlayerPawn.Get();
			if (pawn is null) continue;
			if (!pawn!.IsValid) continue;

			var currentWeapon = pawn!.WeaponServices!.ActiveWeapon.Get();
			if (currentWeapon is null) return HookResult.Continue;
			if (WeaponUtils.IgnoreWeapons.Contains(currentWeapon.DesignerName))
				return HookResult.Continue;

			originPlayer!.DropActiveWeapon();

			CsItem csItem = WeaponUtils.GetRandomWeapon();

			Server.NextFrame(() =>
			{
				originPlayer!.GiveNamedItem(csItem);
				currentWeapon!.Remove();
				if (pawn!.WeaponServices!.ActiveWeapon.Get() is null)
				{
					originPlayer!.GiveNamedItem(CsItem.Knife);
				}
			});
		}

		return HookResult.Continue;
	}
	private readonly BasePlugin.GameEventHandler<EventRoundStart> EventRoundStartHandler = (@event, info) => GiveRandomWeaponForEveryone();
	private readonly BasePlugin.GameEventHandler<EventPlayerHurt> EventPlayerHurtHandler = (@event, info) =>
		{
			if (@event.Userid == null || @event.Attacker is null || @event.Userid.Handle == @event.Attacker.Handle)
				return HookResult.Continue;

			var victim = @event.Userid.OriginalControllerOfCurrentPawn.Get();
			var victimPawn = victim!.PlayerPawn.Get();
			var victimWeapon = victimPawn!.WeaponServices!.ActiveWeapon.Get();

			var attacker = @event.Attacker.OriginalControllerOfCurrentPawn.Get();
			var attackerPawn = attacker!.PlayerPawn.Get();
			var attackerWeapon = attackerPawn!.WeaponServices!.ActiveWeapon.Get();

			if (victimWeapon is null || attackerWeapon is null)
				return HookResult.Continue;


			if (WeaponUtils.IgnoreWeapons.Contains(victimWeapon.DesignerName) || WeaponUtils.IgnoreWeapons.Contains(attackerWeapon.DesignerName))
				return HookResult.Continue;

			string victimWeaponName = victimWeapon.DesignerName;
			string attackerWeaponName = attackerWeapon.DesignerName;

			Server.NextFrame(() =>
			{
				victimPawn.RemovePlayerItem(victimWeapon);
				attackerPawn.RemovePlayerItem(attackerWeapon);


				victim.GiveNamedItem(attackerWeaponName);
				attacker.GiveNamedItem(victimWeaponName);
			});


			return HookResult.Continue;
		};

	public override void OnModeLoad(ElainaServer plugin)
	{
		// try to give random weapon to all players
		GiveRandomWeaponForEveryone();

		plugin.RegisterEventHandler<EventRoundStart>(EventRoundStartHandler);
		plugin.RegisterEventHandler<EventPlayerHurt>(EventPlayerHurtHandler);
	}

	public override void OnModeUnload(ElainaServer plugin)
	{
		plugin.DeregisterEventHandler(EventPlayerHurtHandler, HookMode.Post);
		plugin.DeregisterEventHandler(EventRoundStartHandler, HookMode.Post);
	}
}
