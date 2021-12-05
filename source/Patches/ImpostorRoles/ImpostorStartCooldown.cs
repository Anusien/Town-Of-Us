using System;
using HarmonyLib;

namespace TownOfUs.Patches.ImpostorRoles
{
    /*
     * From the No Hack Too Cheap Department. We can patch the game start event and set the player's cooldown from
     * unintialized zero to the cooldown we want. However, some event after that will run and set the cooldown to
     * 10 for day 1. So what we do is set the cooldown to the value we want. Then we catch when the game attempts to
     * set the starting cooldown to 10 and replace it.
     */
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
    public static class PatchKillTimer
    {
        [HarmonyPriority(Priority.First)]
        public static void Prefix(PlayerControl __instance, ref float time)
        {
            if (
                PlayerControl.GameOptions.KillCooldown > 10
                && __instance.Data.IsImpostor && time == 10
                && (__instance.killTimer > time || __instance.killTimer == 0))
            {
                time = CustomGameOptions.InitialImpostorKillCooldown;
            }
        }
    }
}