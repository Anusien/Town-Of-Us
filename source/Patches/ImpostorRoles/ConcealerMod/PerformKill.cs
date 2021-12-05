using System;
using HarmonyLib;
using Hazel;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Roles;

namespace TownOfUs.Patches.ImpostorRoles.ConcealerMod
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public class PerformKill
    {
        public static bool Prefix(KillButtonManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Concealer))
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

            Concealer role = Role.GetRole<Concealer>(PlayerControl.LocalPlayer);
            if (__instance != role.ConcealButton)
            {
                return true;
            }

            if (
                __instance.isCoolingDown
                || !__instance.isActiveAndEnabled
                || role.ConcealTimer() != 0
                || role.Target == null
                || role.Target.Data.IsImpostor
            )
            {
                return false;
            }

            if (role.Target.isShielded())
            {
                Utils.BreakShield(role.Target);

                if (CustomGameOptions.ShieldBreaks)
                {
                    role.LastConcealed = DateTime.UtcNow;
                }

                return false;
            }

            // Sets concealed player
            role.StartConceal(role.Target);

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.Conceal,
                SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(role.Concealed.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            return false;
        }
    }
}
