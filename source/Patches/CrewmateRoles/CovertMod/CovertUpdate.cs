using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.CovertMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class CovertUpdate
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Covert))
            {
                Covert covert = (Covert) role;
                covert.CovertTick();
            }
        }
    }
}