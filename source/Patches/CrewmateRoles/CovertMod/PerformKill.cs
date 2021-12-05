using HarmonyLib;
using Hazel;
using Rewired;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.CovertMod
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public class PerformKill
    {

        public static bool Prefix(KillButtonManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Covert))
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

            Covert role = Role.GetRole<Covert>(PlayerControl.LocalPlayer);
            if (__instance != role.CovertButton)
            {
                return true;
            }

            if (
                __instance.isCoolingDown
                || !__instance.isActiveAndEnabled
                || role.CovertTimer() != 0
            )
            {
                return false;
            }

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.GoCovert, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            role.GoCovert();
            return false;
        }
    }
}