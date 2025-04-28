using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Cvars;

namespace ElainaServer;

public class TaserWithBhopMode(BaseMode plugin) : BaseMode(plugin)
{
	public override string ModeName => "Zenus Bhop";
	public override string ModeDescription => "In this mode, the taser has no cooldown, and you can use KZ boost jumps!";

	public TaserWithBhopMode() : this(null!)
	{
	}

	private float MpTaserRechargeTime = 0;
	private bool MpEnableBunnyhopping = false;
	private bool MpAutoBunnyhopping = false;
	private float MpAirAccelerate = 0;
	private float MpMaxVelocity = 0;
	private float MpMaxSpeed = 0;
	private float MpStaminaJumpCost = 0;

	public override void OnModeLoad(ElainaServer plugin)
	{
		var Allplayers = Utilities.GetPlayers();

		foreach (var player in Allplayers)
		{
			var originPlayer = player!.OriginalControllerOfCurrentPawn.Get()!;
			if (originPlayer is null) continue;

			PlayerUtils playerUtils = new (originPlayer);

			playerUtils.DropAllWeapons();
			playerUtils.GiveZeus();
		}

		BuyServiceUtils.DisableBuyService();

		MpTaserRechargeTime = ConVar.Find("mp_taser_recharge_time")!.GetPrimitiveValue<float>();
		MpEnableBunnyhopping = ConVar.Find("sv_enablebunnyhopping")!.GetPrimitiveValue<Boolean>();
		MpAutoBunnyhopping = ConVar.Find("sv_autobunnyhopping")!.GetPrimitiveValue<Boolean>();
		MpAirAccelerate = ConVar.Find("sv_airaccelerate")!.GetPrimitiveValue<float>();
		MpMaxVelocity = ConVar.Find("sv_maxvelocity")!.GetPrimitiveValue<float>();
		MpMaxSpeed = ConVar.Find("sv_maxspeed")!.GetPrimitiveValue<float>();
		MpStaminaJumpCost = ConVar.Find("sv_staminajumpcost")!.GetPrimitiveValue<float>();

		ConVar.Find("mp_taser_recharge_time")!.SetValue(0f);
		ConVar.Find("sv_enablebunnyhopping")!.SetValue(true);
		ConVar.Find("sv_autobunnyhopping")!.SetValue(true);
		ConVar.Find("sv_airaccelerate")!.SetValue(99999f);
		ConVar.Find("sv_maxvelocity")!.SetValue(99999f);
		ConVar.Find("sv_maxspeed")!.SetValue(99999f);
		ConVar.Find("sv_staminajumpcost")!.SetValue(0f);
	}

	public override void OnModeUnload(ElainaServer plugin)
	{
		BuyServiceUtils.EnableBuyService();

		ConVar.Find("mp_taser_recharge_time")!.SetValue(MpTaserRechargeTime);
		ConVar.Find("sv_enablebunnyhopping")!.SetValue(MpEnableBunnyhopping);
		ConVar.Find("sv_autobunnyhopping")!.SetValue(MpAutoBunnyhopping);
		ConVar.Find("sv_airaccelerate")!.SetValue(MpAirAccelerate);
		ConVar.Find("sv_maxvelocity")!.SetValue(MpMaxVelocity);
		ConVar.Find("sv_maxspeed")!.SetValue(MpMaxSpeed);
		ConVar.Find("sv_staminajumpcost")!.SetValue(MpStaminaJumpCost);
	}
}
