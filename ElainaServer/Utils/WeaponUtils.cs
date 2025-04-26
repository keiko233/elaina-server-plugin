using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace ElainaServer;
public class WeaponUtils
{
	public readonly static List<string> IgnoreWeapons = ["weapon_c4", "weapon_knife", "weapon_knife_t"];

	public static CsItem GetRandomWeapon()
	{
		Random random = new();

		var guntype = random.Next(2, 5);
		int guntypeKey = 0;
		switch (guntype)
		{
			case 2:
				{
					guntypeKey = random.Next(0, 10);
					break;
				}
			case 3:
				{
					guntypeKey = random.Next(0, 13);
					break;
				}
			case 4:
				{
					guntypeKey = random.Next(0, 11);
					break;
				}
		}

		return (CsItem)(guntype * 100 + guntypeKey);
	}

}
