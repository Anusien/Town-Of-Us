using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.CrewmateRoles.LighterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ManageLighterButton
    {
        private static Sprite Sprite => TownOfUs.LighterSprite;

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

            Lighter role = Role.GetRole<Lighter>(PlayerControl.LocalPlayer);

            if (role.LighterButton == null)
            {
                role.LighterButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.LighterButton.graphic.enabled = true;
            }

            role.LighterButton.graphic.sprite = Sprite;
            role.LighterButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            if (role.IsLighting)
            {
                role.LighterButton.SetCoolDown(role.LighterTimeRemaining, CustomGameOptions.CovertDuration);
                return;
            }

            role.LighterButton.SetCoolDown(role.CooldownTimer(), CustomGameOptions.LighterCooldown);
            role.LighterButton.graphic.color = Palette.EnabledColor;
            role.LighterButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}
