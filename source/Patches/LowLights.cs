using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class LowLights
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player,
            ref float __result)
        {
            if (player == null || player.IsDead)
            {
                __result = __instance.MaxLightRadius;
                return false;
            }

            SwitchSystem switchSystem = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

            if (
                player.IsImpostor()
                || player._object.Is(RoleEnum.Glitch)
                || (IsLighterAndLit(player) && !switchSystem.IsActive)
                )
            {
                __result = __instance.MaxLightRadius * PlayerControl.GameOptions.ImpostorLightMod;
                if (player.Object.Is(ModifierEnum.ButtonBarry))
                    if (Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer).ButtonUsed)
                        __result *= 0.5f;
                return false;
            }

            float lightPercentage = switchSystem.Value / 255f;
            if (player._object.Is(ModifierEnum.Torch) || IsLighterAndLit(player))
            {
                lightPercentage = 1;
            }
            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, lightPercentage) *
                       PlayerControl.GameOptions.CrewLightMod;

            if (player.Object.Is(ModifierEnum.ButtonBarry))
                if (Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer).ButtonUsed)
                    __result *= 0.5f;

            if (player.Object.Is(RoleEnum.Covert) && Role.GetRole<Covert>(PlayerControl.LocalPlayer).IsCovert)
            {
                __result *= 0.5f;
            }
            return false;
        }

        private static bool IsLighterAndLit(GameData.PlayerInfo player)
        {
            if (!player._object.Is(RoleEnum.Lighter))
            {
                return false;
            }

            Lighter lighter = Role.GetRole<Lighter>(player._object);
            return lighter.IsLighting;
        }
    }
}