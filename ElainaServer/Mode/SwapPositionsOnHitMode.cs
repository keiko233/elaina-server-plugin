using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace ElainaServer;

public class SwapPositionsOnHitMode(BaseMode plugin) : BaseMode(plugin)
{
	public override string ModeLocalizerId => "swap_positions_on_hit";

	public SwapPositionsOnHitMode() : this(null!)
	{
	}

	private readonly BasePlugin.GameEventHandler<EventPlayerHurt> EventPlayerHurtHandler = (@event, info) =>
	{
		if (@event is null) return HookResult.Continue;
		if (@event.Attacker is null) return HookResult.Continue;
		if (@event.Userid is null) return HookResult.Continue;

		if (@event.Userid == @event.Attacker) return HookResult.Continue;

		var attacker = @event.Attacker.OriginalControllerOfCurrentPawn.Get()!.PlayerPawn.Get();
		var victim = @event.Userid.OriginalControllerOfCurrentPawn.Get()!.PlayerPawn.Get();

		Vector PositionAttacker = new(attacker!.AbsOrigin!.X, attacker.AbsOrigin.Y, attacker.AbsOrigin.Z);
		Vector PositionVictim = new(victim!.AbsOrigin!.X, victim.AbsOrigin.Y, victim.AbsOrigin.Z);

		Server.NextFrame(() =>
		{
			victim.Teleport(PositionAttacker);
			attacker.Teleport(PositionVictim);
		});

		return HookResult.Continue;
	};

	public override void OnModeLoad(ElainaServer plugin)
	{
		plugin.RegisterEventHandler(EventPlayerHurtHandler);
	}

	public override void OnModeUnload(ElainaServer plugin)
	{
		plugin.DeregisterEventHandler(EventPlayerHurtHandler, HookMode.Post);
	}
}
