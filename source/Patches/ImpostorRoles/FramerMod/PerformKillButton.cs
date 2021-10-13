using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor;
using TownOfUs.Roles;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.FramerMod
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButtonManager __instance)
        {
            if (
                !PlayerControl.LocalPlayer.Is(RoleEnum.Framer)
                || !PlayerControl.LocalPlayer.CanMove
                || PlayerControl.LocalPlayer.Data.IsDead
                || !__instance.isActiveAndEnabled
                || __instance.CurrentTarget == null
                )
            {
                return false;
            }

            Framer role = Role.GetRole<Framer>(PlayerControl.LocalPlayer);
            PlayerControl target = __instance.CurrentTarget;
            role.ClosestPlayer = target;

            PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"Framer is setting {target.nameText} as the target.");

            return true;
        }

        public static void Postfix(KillButtonManager __instance)
        {
            if (
                !PlayerControl.LocalPlayer.Is(RoleEnum.Framer)
                || !PlayerControl.LocalPlayer.CanMove
                || PlayerControl.LocalPlayer.Data.IsDead
                )
            {
                return;
            }

            Framer role = Role.GetRole<Framer>(PlayerControl.LocalPlayer);
            PlayerControl target = role.ClosestPlayer;

            PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"Framer is trying to clean up {target.nameText}");

            DeadBody body = Object.FindObjectsOfType<DeadBody>()
                .FirstOrDefault(b => b.ParentId == target.PlayerId);
            body.enabled = false;

            DateTime deadline = DateTime.Now.AddSeconds(CustomGameOptions.FramerResurfaceTime);

            role.BodiesWaiting.AddItem(new Framer.QueueEntry(body, deadline));

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.FramerDisappear, SendOption.Reliable, -1);
            writer.Write(target.PlayerId);

            role.ClosestPlayer = null;
        }
    }
}