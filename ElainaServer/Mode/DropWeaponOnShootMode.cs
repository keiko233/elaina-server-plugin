using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace ElainaServer;

public class DropWeaponOnShootMode(BaseMode plugin) : BaseMode(plugin)
{
	public override string ModeLocalizerId => "drop_weapon_on_shoot";

	private readonly BasePlugin.GameEventHandler<EventBulletImpact> EventBulletImpactHandler = (@event, info) =>
		{
			var player = @event.Userid!.OriginalControllerOfCurrentPawn.Get();
			var pawn = player!.PlayerPawn.Get();

			PlayerUtils playerUtils = new(player);

			playerUtils.DropWeapon();

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
