using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    internal static class TaskPatches
    {
        [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
        private class GameData_RecomputeTaskCounts
        {
            private static bool Prefix(GameData __instance)
            {
                __instance.TotalTasks = 0;
                __instance.CompletedTasks = 0;
                for (var i = 0; i < __instance.AllPlayers.Count; i++)
                {
                    GameData.PlayerInfo playerInfo = __instance.AllPlayers.ToArray()[i];
                    if (
                        !playerInfo.Disconnected
                        && playerInfo.Tasks != null
                        && playerInfo.Object
                        && (PlayerControl.GameOptions.GhostsDoTasks || !playerInfo.IsDead)
                        && Role.GetRole(playerInfo.Object).Faction == Faction.Crewmates
                    )
                    {
                        for (var j = 0; j < playerInfo.Tasks.Count; j++)
                        {
                            __instance.TotalTasks++;
                            if (playerInfo.Tasks.ToArray()[j].Complete) __instance.CompletedTasks++;
                        }
                    }
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
        private class Console_CanUse
        {
            private static bool Prefix(Console __instance, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref float __result)
            {
                var playerControl = playerInfo.Object;

                // If the console is not a sabotage repair console
                if (
                       Role.GetRole(playerControl).Faction == Faction.Neutral
                       && !playerControl.Is(RoleEnum.Phantom)
                       && !__instance.AllowImpostor
                )
                {
                    __result = float.MaxValue;
                    return false;
                }

                return true;
            }
        }
    }
}
