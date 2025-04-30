using CounterStrikeSharp.API.Core;

namespace ElainaServer;

public class ChangeWeaponOnShootMode(BaseMode plugin) : BaseMode(plugin)
{
	public override string ModeLocalizerId => "change_weapon_on_shoot";

	public ChangeWeaponOnShootMode() : this(null!)
	{
	}

	private readonly BasePlugin.GameEventHandler<EventBulletImpact> EventBulletImpactHandler = (@event, info) =>
	{
		if (@event.Userid == null) return HookResult.Continue;
		var player = @event.Userid.OriginalControllerOfCurrentPawn.Get();

		PlayerUtils playerUtils = new(player);

		playerUtils.DropAndChangeWeapon(WeaponUtils.GetRandomWeapon());

		return HookResult.Continue;
	};

	public override void OnModeLoad(ElainaServer plugin)
	{
		plugin.RegisterEventHandler(EventBulletImpactHandler);
	}

	public override void OnModeUnload(ElainaServer plugin)
	{
		plugin.DeregisterEventHandler(EventBulletImpactHandler, HookMode.Post);
	}
}
