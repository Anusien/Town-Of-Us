using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.Patches.ImpostorRoles.ConcealerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ConcealUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Concealer))
            {
                Concealer concealer = (Concealer) role;
                concealer.ConcealTick();
            }
        }
    }
}
