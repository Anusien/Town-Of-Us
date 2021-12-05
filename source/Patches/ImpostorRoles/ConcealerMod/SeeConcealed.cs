using HarmonyLib;
using Rewired;
using TownOfUs.ImpostorRoles.CamouflageMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.ImpostorRoles.ConcealerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class SeeConcealed
    {
        public static void Postfix(HudManager __instance)
        {
            if (
                PlayerControl.AllPlayerControls.Count <= 1
                || PlayerControl.LocalPlayer == null
                || PlayerControl.LocalPlayer.Data == null
                // Impostors and dead people see them as concealed
                || !(PlayerControl.LocalPlayer.Data.IsImpostor || PlayerControl.LocalPlayer.Data.IsDead)
                || MeetingHud.Instance != null
            )
            {
                return;
            }

            foreach (Concealer role in Role.GetRoles(RoleEnum.Concealer))
            {
                PlayerControl concealed = role.Concealed;
                if (concealed == null)
                {
                    return;
                }

                concealed.nameText.transform.localPosition = new Vector3(0f, 2f, -0.5f);
                concealed.nameText.color = Color.magenta; // TODO is there a better color?
                concealed.nameText.text =
                    (CamouflageUnCamouflage.IsCamoed ? "" : concealed.name) +
                    (role.TimeBeforeConcealed > 0 ? "(Concealing)" : "(Concealed)");
            }
        }
    }
}
