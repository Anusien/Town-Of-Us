using HarmonyLib;
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

            if (player.IsImpostor || player._object.Is(RoleEnum.Glitch))
            {
                __result = __instance.MaxLightRadius * PlayerControl.GameOptions.ImpostorLightMod;
                if (player.Object.Is(ModifierEnum.ButtonBarry))
                    if (Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer).ButtonUsed)
                        __result *= 0.5f;
                return false;
            }

            SwitchSystem switchSystem = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
            float lightPercentage = switchSystem.Value / 255f;
            if (player._object.Is(ModifierEnum.Torch)) lightPercentage = 1;
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
    }
}