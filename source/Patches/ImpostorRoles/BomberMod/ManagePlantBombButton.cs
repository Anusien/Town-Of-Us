using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.ImpostorRoles.BomberMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ManagePlantBombButton
    {
        // TODO
        private static Sprite BombSprite => TownOfUs.ButtonSprite;

        public static void Postfix(HudManager __instance)
        {
            if (
                PlayerControl.AllPlayerControls.Count <= 1
                || PlayerControl.LocalPlayer == null
                || PlayerControl.LocalPlayer.Data == null
                || !PlayerControl.LocalPlayer.Is(RoleEnum.Bomber)
            )
            {
                return;
            }

            Bomber role = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);
            if (role.PlantBombButton == null)
            {
                role.PlantBombButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.PlantBombButton.graphic.enabled = true;
                role.PlantBombButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                role.PlantBombButton.gameObject.SetActive(false);
            }

            role.PlantBombButton.GetComponent<AspectPosition>().Update();
            role.PlantBombButton.graphic.sprite = BombSprite;
            role.PlantBombButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            if (role.IsReadyToPlant())
            {
                Utils.SetTarget(ref role.Target, role.PlantBombButton);
                role.PlantBombButton.SetCoolDown(role.CooldownTimer(), CustomGameOptions.BomberCooldown);
                if (
                    role.Target != null
                    && !role.Target.Data.IsImpostor()
                )
                {
                    role.PlantBombButton.graphic.color = Palette.DisabledClear;
                    role.PlantBombButton.graphic.material.SetFloat("_Desat", 1f);
                }
                else
                {
                    role.PlantBombButton.graphic.color = Palette.EnabledColor;
                    role.PlantBombButton.graphic.material.SetFloat("_Desat", 0f);
                }
            }
            else
            {
                role.PlantBombButton.SetCoolDown(role.TimeUntilBombArmed, CustomGameOptions.BombArmTime);
                role.PlantBombButton.graphic.color = Palette.EnabledColor;
                role.PlantBombButton.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }
}