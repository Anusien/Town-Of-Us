using HarmonyLib;
using Hazel;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.Patches.ImpostorRoles.BomberMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PlantBomb
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

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Bomber))
            {
                return true;
            }

            Bomber bomber = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);
            if (__instance != bomber.PlantBombButton)
            {
                return true;
            }

            if (
                __instance.isCoolingDown
                || !__instance.isActiveAndEnabled
                || bomber.CooldownTimer() != 0
                || bomber.Target == null
                || bomber.Target.Data.IsImpostor()
            )
            {
                return false;
            }

            if (bomber.Target.isShielded())
            {
                Utils.BreakShield(bomber.Target);

                if (CustomGameOptions.ShieldBreaks)
                {
                    bomber.ResetCooldownTimer();
                }

                return false;
            }

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.PlantBomb,
                SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(bomber.Target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            bomber.PlantBomb(bomber.Target);

            return false;
        }
    }
}