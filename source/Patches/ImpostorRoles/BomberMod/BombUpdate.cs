using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.Patches.ImpostorRoles.BomberMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class BombUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (Bomber bomber in Role.GetRoles(RoleEnum.Bomber).Cast<Bomber>())
            {
                bomber.BombTick();
            }
        }
    }
}