using HarmonyLib;
using Hazel;
using TownOfUs.Roles;

namespace TownOfUs.Patches.CrewmateRoles.LighterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Lighter))
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

            Lighter role = Role.GetRole<Lighter>(PlayerControl.LocalPlayer);
            if (__instance != role.LighterButton)
            {
                return true;
            }

            if (
                __instance.isCoolingDown
                || !__instance.isActiveAndEnabled
                || role.CooldownTimer() != 0
            )
            {
                return false;
            }

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.LighterOn, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            role.LightOn();
            return false;
        }
    }
}
