using HarmonyLib;
using Hazel;
using TownOfUs.CrewmateRoles.AltruistMod;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.LoversMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class Die
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] DeathReason reason)
        {
            __instance.Data.IsDead = true;


            var flag3 = __instance.isLover() && CustomGameOptions.BothLoversDie;
            if (!flag3) return true;
            var otherLover = Role.GetRole<Lover>(__instance).OtherLover;
            if (otherLover.Player.Data.IsDead) return true;

            if (reason == DeathReason.Exile)
            {   
                // Rpc call for voted lover
                if (CustomGameOptions.VotedLover && AmongUsClient.Instance.AmHost)
                {
                    otherLover.Voted = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.VotedLover, SendOption.Reliable, -1);
                    writer.Write(otherLover.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                KillButtonTarget.DontRevive = __instance.PlayerId;
            }

            if (AmongUsClient.Instance.AmHost) Utils.RpcMurderPlayer(otherLover.Player, otherLover.Player);

            return true;
        }
    }
}