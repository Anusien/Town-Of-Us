using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.TeleporterMod
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public static class PerformKill
    {
        public static bool Prefix(KillButtonManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Teleporter))
            {
                return true;
            }

            if (
                !PlayerControl.LocalPlayer.CanMove
                || PlayerControl.LocalPlayer.Data.IsDead
            )
            {
                return false;
            }

            Teleporter role = Role.GetRole<Teleporter>(PlayerControl.LocalPlayer);
            if (__instance != role.TeleportButton)
            {
                return true;
            }

            if (
                __instance.isCoolingDown
                || !__instance.isActiveAndEnabled
                || role.TeleportTimer() > 0
                || Utils.IsSabotageActive()
            )
            {
                return false;
            }

            role.Teleport();
            return false;
        }
    }
}
