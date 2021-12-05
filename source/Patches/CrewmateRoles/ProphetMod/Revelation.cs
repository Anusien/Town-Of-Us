using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.ProphetMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class Revelation
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (
                PlayerControl.AllPlayerControls.Count <= 1
                || PlayerControl.LocalPlayer == null
                || PlayerControl.LocalPlayer.Data == null
                || MeetingHud.Instance
                || !PlayerControl.LocalPlayer.CanMove
                || !PlayerControl.LocalPlayer.Is(RoleEnum.Prophet)
                || PlayerControl.LocalPlayer.Data.IsDead
                )
            {
                return;
            }

            if (!IsTimeForRevelation())
            {
                return;
            }

            Role.GetRole<Prophet>(PlayerControl.LocalPlayer).Revelation();
        }

        private static bool IsTimeForRevelation()
        {
            var now = DateTime.UtcNow;

            var role = Role.GetRole<Prophet>(PlayerControl.LocalPlayer);
            var timeSpan = now - role.LastRevealed;
            var cooldown = CustomGameOptions.ProphetCooldown * 1000f;

            return cooldown <= timeSpan.TotalMilliseconds;
        }
    }
}