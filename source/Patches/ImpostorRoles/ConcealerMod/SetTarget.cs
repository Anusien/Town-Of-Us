using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.Patches.ImpostorRoles.ConcealerMod
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.SetTarget))]
    public class SetTarget
    {
        public static void Postfix(KillButtonManager __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (
                PlayerControl.AllPlayerControls.Count <= 1
                || PlayerControl.LocalPlayer == null
                || PlayerControl.LocalPlayer.Data == null
                || !PlayerControl.LocalPlayer.Is(RoleEnum.Concealer)
                || target == null
            )
            {
                return;
            }
            Concealer role = Role.GetRole<Concealer>(PlayerControl.LocalPlayer);
            if (__instance != role.ConcealButton)
            {
                return;
            }

            if (target.Data.IsImpostor)
            {
                __instance.renderer.color = Palette.DisabledClear;
                __instance.renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}
