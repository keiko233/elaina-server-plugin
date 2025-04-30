namespace ElainaServer;

public class SpeedAndPassisonMode(BaseMode plugin) : BaseMode(plugin)
{
	public override string ModeLocalizerId => "speed_and_passion";
	// public override string ModeName => "Speed and Passion Mode";
	// public override string ModeDescription => "Move faster, jump higher with lower gravity!";

	public SpeedAndPassisonMode() : this(null!)
	{
	}

	public override void OnModeLoad(ElainaServer plugin)
	{
		throw new NotImplementedException();
	}

	public override void OnModeUnload(ElainaServer plugin)
	{
		throw new NotImplementedException();
	}
}
