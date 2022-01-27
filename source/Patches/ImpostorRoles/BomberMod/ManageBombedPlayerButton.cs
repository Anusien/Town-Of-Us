using System.Linq;
using HarmonyLib;
using Rewired;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.ImpostorRoles.BomberMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ManageBombedPlayerButton
    {
        public static void Postfix(HudManager __instance)
        {
            if (
                PlayerControl.AllPlayerControls.Count <= 1
                || PlayerControl.LocalPlayer == null
                || PlayerControl.LocalPlayer.Data == null
            )
            {
                return;
            }

            foreach (var bomber in Role.GetRoles(RoleEnum.Bomber).ToList().Cast<Bomber>())
            {
                if (bomber.KillWithBombButton == null)
                {
                    bomber.KillWithBombButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    bomber.KillWithBombButton.graphic.enabled = true;
                    // TODO
                    bomber.KillWithBombButton.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(
                        Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f,
                        __instance.UseButton.transform.position.y, __instance.UseButton.transform.position.z);
                    bomber.KillWithBombButton.gameObject.SetActive(false);
                }
                bomber.KillWithBombButton.GetComponent<AspectPosition>().Update();
                bomber.KillWithBombButton.graphic.sprite = TranslationController.Instance.GetImage(ImageNames.KillButton);
                bomber.KillWithBombButton.gameObject.SetActive(
                    !PlayerControl.LocalPlayer.Data.IsDead
                    && !MeetingHud.Instance
                    && bomber.ShouldShowKillWithBombButton(PlayerControl.LocalPlayer)
                );

                bomber.KillWithBombButton.SetCoolDown(0, 0); // TODO
            }
        }
    }
}