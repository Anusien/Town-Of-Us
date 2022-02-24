using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.ProphetMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class Revelation
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!PlayerControl.LocalPlayer.Data.IsDead
                && PlayerControl.LocalPlayer.Is(RoleEnum.Prophet)
                && ShouldReveal(__instance))
            {
                Role.GetRole<Prophet>(PlayerControl.LocalPlayer).Revelation();
            }
        }

        private static bool ShouldReveal(PlayerControl __instance)
        {
            var taskInfos = __instance.Data.Tasks.ToArray();
            var allTasksCount = taskInfos.Count;
            var maxRevealsCount = 
                CustomGameOptions.ProphetTotalReveals < 1 ? 1 : CustomGameOptions.ProphetTotalReveals;
            var currentRevealsCount = Role.GetRole<Prophet>(PlayerControl.LocalPlayer).Revealed.Count;

            var requiredTasksForNextReveal = (currentRevealsCount + 1)
                                             * ((float) allTasksCount / maxRevealsCount);

            var completedTasksCount = taskInfos.Count(x => x.Complete);
            return completedTasksCount >= requiredTasksForNextReveal;
        }
    }
}