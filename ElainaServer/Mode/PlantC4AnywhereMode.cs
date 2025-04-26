using CounterStrikeSharp.API.Modules.Cvars;

namespace ElainaServer;

public class PlantC4AnywhereMode(BaseMode plugin) : BaseMode(plugin)
{
	public override string ModeName => "Plant C4 Anywhere Mode";
	public override string ModeDescription => "You can plant the bomb anywhere, but heads up — the C4 timer is extended to 100 seconds. Good luck!";

	public PlantC4AnywhereMode() : this(null!)
	{
	}

	private int ExtendedC4timer = 100;
	private bool MpPlantC4Anywhere = false;
	private int MpC4timer = 0;

	public override void OnModeLoad(ElainaServer plugin)
	{
		MpPlantC4Anywhere = ConVar.Find("mp_plant_c4_anywhere")!.GetPrimitiveValue<Boolean>();
		MpC4timer = ConVar.Find("mp_c4timer")!.GetPrimitiveValue<Int32>();

		ConVar.Find("mp_plant_c4_anywhere")!.SetValue(true);
		ConVar.Find("mp_c4timer")!.SetValue(ExtendedC4timer);
	}

	public override void OnModeUnload(ElainaServer plugin)
	{
		ConVar.Find("mp_plant_c4_anywhere")!.SetValue(MpPlantC4Anywhere);
		ConVar.Find("mp_c4timer")!.SetValue(MpC4timer);
	}
}
