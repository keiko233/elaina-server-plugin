using CounterStrikeSharp.API.Modules.Cvars;

namespace ElainaServer;

public class BuyServiceUtils
{
	private static int SvBuyStatusOverride = 0;
	private static float MpBuytime = 0;
	private static int MpBuyAnywhere = 0;
	private static bool MpBuyAllowGrenades = false;
	private static int MpBuyAllowGuns = 0;


	public static void DisableBuyService()
	{
		SvBuyStatusOverride = ConVar.Find("sv_buy_status_override")!.GetPrimitiveValue<Int32>();
		MpBuytime = ConVar.Find("mp_buytime")!.GetPrimitiveValue<float>();
		MpBuyAnywhere = ConVar.Find("mp_buy_anywhere")!.GetPrimitiveValue<Int32>();
		MpBuyAllowGrenades = ConVar.Find("mp_buy_allow_grenades")!.GetPrimitiveValue<bool>();
		MpBuyAllowGuns = ConVar.Find("mp_buy_allow_guns")!.GetPrimitiveValue<Int32>();

		// Disable the buy service
		ConVar.Find("mp_buytime")!.SetValue(0f);
		ConVar.Find("mp_buy_anywhere")!.SetValue(0);
		ConVar.Find("mp_buy_allow_grenades")!.SetValue(false);
		ConVar.Find("mp_buy_allow_guns")!.SetValue(0);
	}

	public static void EnableBuyService()
	{
		// Enable the buy service
		ConVar.Find("sv_buy_status_override")!.SetValue(SvBuyStatusOverride);
		ConVar.Find("mp_buytime")!.SetValue(MpBuytime);
		ConVar.Find("mp_buy_anywhere")!.SetValue(MpBuyAnywhere);
		ConVar.Find("mp_buy_allow_grenades")!.SetValue(MpBuyAllowGrenades);
		ConVar.Find("mp_buy_allow_guns")!.SetValue(MpBuyAllowGuns);
	}
}
