using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace ElainaServer;

public class DropWeaponOnShootMode(BaseMode plugin) : BaseMode(plugin)
{
	public override string ModeName => "Drop Weapon On Shoot Mode";
	public override string ModeDescription => "When you shoot, your weapon will drop.";

	private BasePlugin.GameEventHandler<EventBulletImpact> EventBulletImpactHandler = (@event, info) =>
		{
			var player = @event.Userid!.OriginalControllerOfCurrentPawn.Get();
			var pawn = player!.PlayerPawn.Get();

			var currentWeapon = pawn!.WeaponServices!.ActiveWeapon.Get();
			if (currentWeapon is null) return HookResult.Continue;
			if (WeaponUtils.IgnoreWeapons.Contains(currentWeapon.DesignerName))
				return HookResult.Continue;

			player!.DropActiveWeapon();
			Server.NextFrame(() =>
			{
				if (pawn!.WeaponServices!.ActiveWeapon.Get() is null)
				{
					player!.GiveNamedItem(CsItem.Knife);
				}
			});
			return HookResult.Continue;
		};

	public DropWeaponOnShootMode() : this(null!)
	{
	}

	public override void OnModeLoad(ElainaServer plugin)
	{
		plugin.RegisterEventHandler<EventBulletImpact>(EventBulletImpactHandler);
	}

	public override void OnModeUnload(ElainaServer plugin)
	{
		plugin.DeregisterEventHandler<EventBulletImpact>(EventBulletImpactHandler);
	}
}
