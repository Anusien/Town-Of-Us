using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Patches.ImpostorRoles.ConcealerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ManageConcealButton
    {
        public static void Postfix(HudManager __instance)
        {
            if (
                PlayerControl.AllPlayerControls.Count <= 1
                || PlayerControl.LocalPlayer == null
                || PlayerControl.LocalPlayer.Data == null
                || !PlayerControl.LocalPlayer.Is(RoleEnum.Concealer)
            )
            {
                return;
            }

            Concealer role = Role.GetRole<Concealer>(PlayerControl.LocalPlayer);
            if (role.ConcealButton == null)
            {
                role.ConcealButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.ConcealButton.renderer.enabled = true;
            }

            role.ConcealButton.renderer.sprite = TownOfUs.SwoopSprite;

            role.ConcealButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            var position = __instance.KillButton.transform.localPosition;
            role.ConcealButton.transform.localPosition = new Vector3(position.x,
                __instance.ReportButton.transform.localPosition.y, position.z);

            if (role.Concealed != null)
            {
                // TODO: This will kind of lie to them about how long the conceal lasts, can we change the experience?
                role.ConcealButton.SetCoolDown(role.TimeBeforeConcealed + role.ConcealTimeRemaining, CustomGameOptions.ConcealDuration);
                return;
            }

            Utils.SetTarget(ref role.Target, role.ConcealButton);

            role.ConcealButton.SetCoolDown(role.ConcealTimer(), CustomGameOptions.ConcealCooldown);
            role.ConcealButton.renderer.color = Palette.EnabledColor;
        }
    }
}
