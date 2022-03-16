using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.Patches.CrewmateRoles.LighterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class LighterUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Lighter))
            {
                Lighter lighter = (Lighter) role;
                lighter.LightTick();
            }
        }
    }
}
