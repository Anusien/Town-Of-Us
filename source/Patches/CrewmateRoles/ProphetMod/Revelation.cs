using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.ProphetMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class Revelation
    {
        public static void Postfix()
        {
            if (!PlayerControl.LocalPlayer.Data.IsDead
                && PlayerControl.LocalPlayer.Is(RoleEnum.Prophet)
                && ShouldReveal())
            {
                Role.GetRole<Prophet>(PlayerControl.LocalPlayer).Revelation();
            }
        }

        private static bool ShouldReveal()
        {
            var taskInfos = PlayerControl.LocalPlayer.Data.Tasks.ToArray();
            var allTasksCount = taskInfos.Count;
            var maxRevealsCount = CustomGameOptions.ProphetTotalReveals;
            var currentRevealsCount = Role.GetRole<Prophet>(PlayerControl.LocalPlayer).Revealed.Count;

            var completedTasksCount = taskInfos.Count(x => x.Complete);
            return completedTasksCount * maxRevealsCount >= allTasksCount * (currentRevealsCount + 1);
        }
    }
}