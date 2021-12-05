using System;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TownOfUs.CrewmateRoles.SeerMod
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public class PerformKill
    {
        public static bool Prefix(KillButtonManager __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer)) return true;
            var role = Role.GetRole<Seer>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            if (role.SeerTimer() != 0f) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            var targetId = role.ClosestPlayer.PlayerId;

            var successfulSee = CheckSeerChance(role.ClosestPlayer);

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.Investigate, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(targetId);
            writer.Write(successfulSee);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            role.LastInvestigated = DateTime.UtcNow;
            role.Investigated.Add(role.ClosestPlayer.PlayerId, successfulSee);

            return false;
        }

        private static bool CheckSeerChance(PlayerControl target)
        {
            float chance;
            switch (Role.GetRole(target).Faction)
            {
                case Faction.Crewmates:
                    chance = CustomGameOptions.SeerCrewmateChance;
                    break;
                case Faction.Neutral:
                    chance = CustomGameOptions.SeerNeutralChance;
                    break;
                case Faction.Impostors:
                default:
                    chance = CustomGameOptions.SeerImpostorChance;
                    break;
            }

            var seen = Random.RandomRangeInt(1, 101) <= chance;
            return seen;
        }
    }
}
