using System;
using HarmonyLib;
using Reactor.Extensions;
using TMPro;
using TownOfUs.Extensions;
using TownOfUs.Patches;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.AssassinMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {

        private static bool IsExempt(PlayerControl player) {
            if (
                player.Data.IsImpostor &&
                (CustomGameOptions.ImpostorsKnowTeam == AnonymousEnum.ImpostorSeeRoles ||
                CustomGameOptions.ImpostorsKnowTeam == AnonymousEnum.ImpostorSeeTeam) //allow assassin to guess his mate if he don't know their team
            )
            {
                return true;
            }

            var role = Role.GetRole(player);
            return role != null && role.Criteria();
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Assassin))
            {
                var assassin = (Assassin) role;
                assassin.Guesses.Clear();
                assassin.Buttons.Clear();
            }

            if (
                PlayerControl.LocalPlayer.Data.IsDead
                || !PlayerControl.LocalPlayer.Is(RoleEnum.Assassin)
            )
            {
                return;
            }

            var assassinRole = Role.GetRole<Assassin>(PlayerControl.LocalPlayer);
            if (assassinRole.RemainingKills <= 0) return;
            foreach (var voteArea in __instance.playerStates)
            {
                IMeetingGuesser.GenButton(
                    assassinRole,
                    voteArea,
                    playerControl => !IsExempt(playerControl),
                    (playerControl, role) =>
                    {
                        IMeetingGuesser.KillFromMeetingGuess(assassinRole, playerControl, role);
                        assassinRole.RemainingKills--;
                    });
            }
        }
    }
}
