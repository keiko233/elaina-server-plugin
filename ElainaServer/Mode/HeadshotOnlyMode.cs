using CounterStrikeSharp.API.Modules.Cvars;

namespace ElainaServer;

public class HeadshotOnlyMode(BaseMode plugin) : BaseMode(plugin)
{
	public override string ModeLocalizerId => "headshot_only";

	public HeadshotOnlyMode() : this(null!)
	{
	}

	private bool MpDamageHeadshotOnly = false;

	public override void OnModeLoad(ElainaServer plugin)
	{
		MpDamageHeadshotOnly = ConVar.Find("mp_damage_headshot_only")!.GetPrimitiveValue<bool>();
		ConVar.Find("mp_damage_headshot_only")!.SetValue(true);
	}

	public override void OnModeUnload(ElainaServer plugin)
	{
		ConVar.Find("mp_damage_headshot_only")!.SetValue(MpDamageHeadshotOnly);
	}
}
