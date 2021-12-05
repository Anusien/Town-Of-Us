using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.CovertMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ManageCovertButton
    {
        private static Sprite CovertSprite => TownOfUs.SwoopSprite;

        public static void Postfix(HudManager __instance)
        {
            if (
                PlayerControl.AllPlayerControls.Count <= 1
                || PlayerControl.LocalPlayer == null
                || PlayerControl.LocalPlayer.Data == null
                || !PlayerControl.LocalPlayer.Is(RoleEnum.Covert)
            )
            {
                return;
            }

            Covert role = Role.GetRole<Covert>(PlayerControl.LocalPlayer);

            if (role.CovertButton == null)
            {
                role.CovertButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.CovertButton.renderer.enabled = true;
            }

            role.CovertButton.renderer.sprite = CovertSprite;
            role.CovertButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            if (role.IsCovert)
            {
                role.CovertButton.SetCoolDown(role.CovertTimeRemaining, CustomGameOptions.CovertDuration);
                return;
            }

            role.CovertButton.SetCoolDown(role.CovertTimer(), CustomGameOptions.CovertCooldown);
            role.CovertButton.renderer.color = Palette.EnabledColor;
            role.CovertButton.renderer.material.SetFloat("_Desat", 0f);
        }
    }
}