using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.TeleporterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (
                PlayerControl.AllPlayerControls.Count <= 1
                || PlayerControl.LocalPlayer == null
                || PlayerControl.LocalPlayer.Data == null
                || !PlayerControl.LocalPlayer.Is(RoleEnum.Teleporter)
            )
            {
                return;
            }

            var role = Role.GetRole<Teleporter>(PlayerControl.LocalPlayer);

            if (role.TeleportButton == null)
            {
                role.TeleportButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.TeleportButton.renderer.enabled = true;
            }

            role.TeleportButton.renderer.sprite = TownOfUs.ButtonSprite;
            role.TeleportButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            var position = __instance.KillButton.transform.localPosition;
            role.TeleportButton.transform.localPosition = new Vector3(position.x,
                __instance.ReportButton.transform.localPosition.y, position.z);
            role.TeleportButton.SetCoolDown(role.TeleportTimer(), CustomGameOptions.TeleporterCooldown);

            if (
                role.TeleportButton.enabled
                && !role.TeleportButton.isCoolingDown
                && !Utils.IsSabotageActive()
                )
            {
                role.TeleportButton.renderer.color = Palette.EnabledColor;
                role.TeleportButton.renderer.material.SetFloat("_Desat", 0f);
                return;
            }

            role.TeleportButton.renderer.color = Palette.DisabledClear;
            role.TeleportButton.renderer.material.SetFloat("_Desat", 1f);
        }
    }
}
