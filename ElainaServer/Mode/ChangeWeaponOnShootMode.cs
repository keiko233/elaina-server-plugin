using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;

namespace ElainaServer;

public class ChangeWeaponOnShootMode(BaseMode plugin) : BaseMode(plugin)
{
	public override string ModeName => "Change Weapon On Shoot";

	public override string ModeDescription => "When you fire, you’ll randomly switch to another weapon. Good luck!";

	public ChangeWeaponOnShootMode() : this(null!)
	{
	}

	private readonly BasePlugin.GameEventHandler<EventBulletImpact> EventBulletImpactHandler = (@event, info) =>
	{
		if (@event.Userid == null) return HookResult.Continue;

		var player = @event.Userid!.OriginalControllerOfCurrentPawn.Get();
		var pawn = player!.PlayerPawn.Get();

		var currentWeapon = pawn!.WeaponServices!.ActiveWeapon.Get();
		if (currentWeapon is null) return HookResult.Continue;
		if (WeaponUtils.IgnoreWeapons.Contains(currentWeapon.DesignerName))
			return HookResult.Continue;

		CsItem randomWeapon = WeaponUtils.GetRandomWeapon();

		player!.DropActiveWeapon();

		player!.GiveNamedItem(randomWeapon);
		Server.NextFrame(() =>
		{
			currentWeapon!.Remove();
			if (pawn!.WeaponServices!.ActiveWeapon.Get() is null)
			{
				player!.GiveNamedItem(CsItem.Knife);
			}
		});

		return HookResult.Continue;
	};

	public override void OnModeLoad(ElainaServer plugin)
	{
		plugin.RegisterEventHandler<EventBulletImpact>(EventBulletImpactHandler);
	}

	public override void OnModeUnload(ElainaServer plugin)
	{
		plugin.DeregisterEventHandler(EventBulletImpactHandler, HookMode.Post);
	}
}
