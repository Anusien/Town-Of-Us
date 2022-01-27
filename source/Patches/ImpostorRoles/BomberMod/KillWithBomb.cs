using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.Patches.ImpostorRoles.BomberMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class KillWithBomb
    {
        public static bool Prefix(KillButton __instance)
        {
            if (
                !PlayerControl.LocalPlayer.CanMove
                || PlayerControl.LocalPlayer.Data.IsDead
            )
            {
                return false;
            }

            foreach (Bomber bomber in Role.GetRoles(RoleEnum.Bomber).ToList().Cast<Bomber>())
            {
                if (
                    __instance != bomber.KillWithBombButton
                    || bomber.ShouldShowKillWithBombButton(PlayerControl.LocalPlayer)
                )
                {
                    continue;
                }

                if (
                    __instance.isCoolingDown
                    || !__instance.isActiveAndEnabled
                    || bomber.BombedPlayerTarget == null
                )
                {
                    return false;
                }

                if (bomber.BombedPlayerTarget.PlayerId == bomber.Player.PlayerId)
                {
                    bomber.BombKill(bomber.BombedPlayer);
                }
                else
                {
                    bomber.BombKill(bomber.BombedPlayerTarget);
                }

                return false;
            }

            return true;
        }
    }
}