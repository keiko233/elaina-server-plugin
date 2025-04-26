using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace ElainaServer;

public class SwitchWeaponWhenHitMode(BaseMode plugin) : BaseMode(plugin)
{
    public override string ModeName => "Use Your Opponent's Weapon When Hit Mode";
    public override string ModeDescription => "When you hit your opponent, exchange weapons with him";
    public SwitchWeaponWhenHitMode() : this(null!)
    {
    }

    private readonly List<string> IgnoreWeapons = ["weapon_c4", "weapon_knife", "weapon_knife_t"];
    private Random random = new Random();
    private BasePlugin.GameEventHandler<EventRoundStart>? EventRoundStartHandler;
    private BasePlugin.GameEventHandler<EventPlayerHurt>? EventPlayerHurtHandler;

    public override void OnModeLoad(ElainaServer plugin)
    {
        var ignoreWeapons = IgnoreWeapons;

        plugin.RegisterEventHandler<EventRoundStart>(EventRoundStartHandler = (@event, info) =>
               {

                   List<CCSPlayerController> Allplayers = Utilities.GetPlayers();

                   foreach (var player in Allplayers)
                   {
                       if (player == null || !player.IsValid) continue;

                       var originPlayers = player!.OriginalControllerOfCurrentPawn.Get()!;
                       if (originPlayers is null) continue;
                       CCSPlayerPawn? pawn = originPlayers.PlayerPawn.Get();
                       if (pawn is null) continue;
                       if (!pawn!.IsValid) continue;
                       var currentWeapon = pawn!.WeaponServices!.ActiveWeapon.Get();
                       if (currentWeapon is null) return HookResult.Continue;
                       if (ignoreWeapons.Contains(currentWeapon.DesignerName))
                           return HookResult.Continue;

                       var guntype = random.Next(2, 5);
                       int guntype_add = 0;
                       switch (guntype)
                       {
                           case 2:
                               {
                                   guntype_add = random.Next(0, 10);
                                   break;
                               }
                           case 3:
                               {
                                   guntype_add = random.Next(0, 13);
                                   break;
                               }
                           case 4:
                               {
                                   guntype_add = random.Next(0, 11);
                                   break;
                               }
                       }
                       CsItem csItem = (CsItem)(guntype * 100 + guntype_add);

                       player!.DropActiveWeapon();

                       player!.GiveNamedItem(csItem);
                       Server.NextFrame(() =>
                       {
                           currentWeapon!.Remove();
                           if (pawn!.WeaponServices!.ActiveWeapon.Get() is null)
                           {
                               player!.GiveNamedItem(CsItem.Knife);
                           }
                       });
                   }

                   return HookResult.Continue;
               });

        plugin.RegisterEventHandler<EventPlayerHurt>(EventPlayerHurtHandler = (@event, info) =>
        {
            if (@event.Userid == null || @event.Attacker is null || @event.Userid.Handle == @event.Attacker.Handle)
                return HookResult.Continue;

            var victim = @event.Userid.OriginalControllerOfCurrentPawn.Get();
            var victimPawn = victim!.PlayerPawn.Get();
            var victimWeapon = victimPawn!.WeaponServices!.ActiveWeapon.Get();

            var attacker = @event.Attacker.OriginalControllerOfCurrentPawn.Get();
            var attackerPawn = attacker!.PlayerPawn.Get();
            var attackerWeapon = attackerPawn!.WeaponServices!.ActiveWeapon.Get();

            if (victimWeapon is null || attackerWeapon is null)
                return HookResult.Continue;


            if (ignoreWeapons.Contains(victimWeapon.DesignerName) || ignoreWeapons.Contains(attackerWeapon.DesignerName))
                return HookResult.Continue;

            string victimWeaponName = victimWeapon.DesignerName;
            string attackerWeaponName = attackerWeapon.DesignerName;

            Server.NextFrame(() =>
            {
                victimPawn.RemovePlayerItem(victimWeapon);
                attackerPawn.RemovePlayerItem(attackerWeapon);


                victim.GiveNamedItem(attackerWeaponName);
                attacker.GiveNamedItem(victimWeaponName);
            });


            return HookResult.Continue;
        });
    }

    public override void OnModeUnload(ElainaServer plugin)
    {
        if (EventPlayerHurtHandler != null)
        {
            plugin.DeregisterEventHandler(EventPlayerHurtHandler, HookMode.Post);
        }

        if (EventRoundStartHandler != null)
        {
            plugin.DeregisterEventHandler(EventRoundStartHandler, HookMode.Post);
        }
    }
}
