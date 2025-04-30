using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace ElainaServer;

public class PlayerUtils
{
	private readonly CCSPlayerController? player;

	private readonly CCSPlayerPawn? pawn;

	private readonly CBasePlayerWeapon? currentWeapon;

	private readonly bool? isIgnoredWeapon;

	public PlayerUtils(CCSPlayerController? _player)
	{
		player = _player;
		pawn = player?.PlayerPawn.Get();
		currentWeapon = pawn!.WeaponServices!.ActiveWeapon.Get();
		isIgnoredWeapon = currentWeapon is null ? null : WeaponUtils.IgnoreWeapons.Contains(currentWeapon.DesignerName);
	}

	private CBasePlayerWeapon? GetCurrentWeapon()
	{
		return pawn!.WeaponServices!.ActiveWeapon.Get();
	}

	public void DropWeapon()
	{
		if (player == null || !player.IsValid) return;

		if (isIgnoredWeapon == true) return;

		player.DropActiveWeapon();
	}

	public void DropAndChangeWeapon(CsItem nextWeapon)
	{
		if (player == null || !player.IsValid) return;

		if (isIgnoredWeapon == true) return;

		// need to drop the current weapon frist
		DropWeapon();

		player.GiveNamedItem(nextWeapon);

		Server.NextFrame(() =>
		{
			// then remove the current weapon
			currentWeapon?.Remove();
			if (GetCurrentWeapon() is null)
			{
				player.GiveNamedItem(CsItem.Knife);
			}
		});
	}

	public void DropAllWeapons()
	{
		if (player == null || !player.IsValid) return;

		player.RemoveWeapons();

		player.GiveNamedItem(CsItem.HE);
		player.GiveNamedItem(CsItem.Knife);
		player.GiveNamedItem(CsItem.Kevlar);
		player.GiveNamedItem(CsItem.KevlarHelmet);
	}

	public void GiveZeus()
	{
		if (player == null || !player.IsValid) return;

		player.GiveNamedItem(CsItem.Zeus);
	}
}
