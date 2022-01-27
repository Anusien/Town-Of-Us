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
                role.ConcealButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ConcealButton.graphic.enabled = true;
                role.ConcealButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                role.ConcealButton.gameObject.SetActive(false);
            }

            role.ConcealButton.GetComponent<AspectPosition>().Update();
            role.ConcealButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            // TODO: Make our own button for this
            role.ConcealButton.graphic.sprite = TownOfUs.SwoopSprite;

            if (role.Concealed != null)
            {
                role.ConcealButton.SetCoolDown(role.TimeBeforeConcealed + role.ConcealTimeRemaining, CustomGameOptions.ConcealDuration);
                return;
            }

            Utils.SetTarget(ref role.Target, role.ConcealButton);

            role.ConcealButton.SetCoolDown(role.CooldownTimer(), CustomGameOptions.ConcealCooldown);
            role.ConcealButton.graphic.color = Palette.EnabledColor;
        }
    }
}
