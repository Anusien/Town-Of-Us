using System;
using HarmonyLib;
using Hazel;
using Reactor;
using Rewired;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.FramerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class ReenableBodies
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (
                PlayerControl.AllPlayerControls.Count <= 1
                || PlayerControl.LocalPlayer == null
                || PlayerControl.LocalPlayer.Data == null
                || !PlayerControl.LocalPlayer.Is(RoleEnum.Framer)
            )
            {
                return;
            }

            PlayerControl player = PlayerControl.LocalPlayer;
            Framer role = Role.GetRole<Framer>(PlayerControl.LocalPlayer);
            DateTime now = DateTime.UtcNow;
            while (role.BodiesWaiting.TryPeek(out Framer.QueueEntry body))
            {
                if (player.Data.IsDead || now >= body.Deadline)
                {
                    PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"{(player.Data.IsDead ? "Framer is dead" : "It's been enough time")}, trying to re-eneable the body for {body.DeadBody.name}");
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.FramerReappear, SendOption.Reliable, -1);
                    writer.Write(body.DeadBody.ParentId);
                    body.DeadBody.enabled = true;
                }
            }
        }
    }
}