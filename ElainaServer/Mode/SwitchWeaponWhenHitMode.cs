using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace ElainaServer;

public class SwitchWeaponWhenHitMode(BaseMode plugin) : BaseMode(plugin)
{
	public override string ModeLocalizerId => "switch_weapon_when_hit";

	public SwitchWeaponWhenHitMode() : this(null!)
	{
	}

	private static HookResult GiveRandomWeaponForEveryone()
	{

		List<CCSPlayerController> Allplayers = Utilities.GetPlayers();

		foreach (var player in Allplayers)
		{
			PlayerUtils playerUtils = new(player);

			playerUtils.DropAndChangeWeapon(WeaponUtils.GetRandomWeapon());
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

		plugin.RegisterEventHandler(EventRoundStartHandler);
		plugin.RegisterEventHandler(EventPlayerHurtHandler);
	}

	public override void OnModeUnload(ElainaServer plugin)
	{
		plugin.DeregisterEventHandler(EventPlayerHurtHandler);
		plugin.DeregisterEventHandler(EventRoundStartHandler);
	}
}
