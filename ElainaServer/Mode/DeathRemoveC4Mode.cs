using CounterStrikeSharp.API.Modules.Cvars;

namespace ElainaServer;

public class DeathRemoveC4Mode(BaseMode plugin) : BaseMode(plugin)
{
	public override string ModeLocalizerId => "death_remove_c4";

    public DeathRemoveC4Mode() : this(null!)
    {
    }

    private bool MpDeathDropC4 = false;

    public override void OnModeLoad(ElainaServer plugin)
    {
        MpDeathDropC4 = ConVar.Find("mp_death_drop_c4")!.GetPrimitiveValue<Boolean>();
        ConVar.Find("mp_death_drop_c4")!.SetValue(true);
    }

    public override void OnModeUnload(ElainaServer plugin)
    {
        ConVar.Find("mp_death_drop_c4")!.SetValue(MpDeathDropC4);
    }
}
