using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Analyst : Role, IMeetingGuesser
    {
        public Dictionary<byte, (GameObject, GameObject, TMP_Text)> Buttons { get; } = new Dictionary<byte, (GameObject, GameObject, TMP_Text)>();
        public Dictionary<byte, int> Guesses { get; } = new Dictionary<byte, int>();
        public List<RoleEnum> PossibleGuesses { get; }
        private int CorrectGuesses { get; set; }
        public bool AnalystWins { get; private set; }

        public Analyst(PlayerControl player) : base(player, RoleEnum.Analyst)
        {
            ImpostorText = () => "Figure out everyone's roles to win";
            TaskText = () => "Guess the roles of everyone during a meeting and win";

            PossibleGuesses = CustomGameOptions.GetEnabledRoles(Faction.Crewmates, Faction.Neutral, Faction.Impostors);
            PossibleGuesses.Add(RoleEnum.Crewmate);
            PossibleGuesses.Add(RoleEnum.Impostor);
            PossibleGuesses.Remove(RoleEnum.Analyst);
        }

        public bool CanKeepGuessing()
        {
            return true;
        }

        protected override void DoOnMeetingEnd()
        {
            CorrectGuesses = 0;
        }

        public void GuessCorrectly()
        {
            CorrectGuesses++;

            if (CorrectGuesses == NumGuessesNeeded())
            {
                Wins();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.ToArray())
                {
                    if (
                        player == null ||
                        player.Data.Disconnected ||
                        player.Data.IsDead
                    ) continue;
                    Utils.RpcMurderPlayer(player, player);
                }

                Utils.RpcMurderPlayer(Player, Player);
            }
        }

        public void Wins()
        {
            AnalystWins = true;
        }

        public void Loses()
        {
            Player.Data.IsImpostor = true;
        }

        private int NumGuessesNeeded()
        {
            if (Player.Data.IsDead)
            {
                // TODO
                return Int32.MaxValue;
            }

            int players = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected);
            return Math.Min(players - 1, 2);
        }

        // RpcEndGame ?
        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (
                Player.Data.IsDead
                || Player.Data.Disconnected
                || CorrectGuesses < NumGuessesNeeded()
            )
            {
                return true;
            }

            Wins();

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.AnalystWin,
                SendOption.Reliable,
                -1
            );
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            Utils.EndGame();
            return false;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Analyst))
            {
                Analyst analystRole = (Analyst) role;
                analystRole.Guesses.Clear();
                analystRole.Buttons.Clear();
            }

            if (
                PlayerControl.LocalPlayer.Data.IsDead
                || !PlayerControl.LocalPlayer.Is(RoleEnum.Analyst)
            )
            {
                return;
            }

            Analyst analyst = Role.GetRole<Analyst>(PlayerControl.LocalPlayer);
            foreach (PlayerVoteArea voteArea in __instance.playerStates)
            {
                IMeetingGuesser.GenButton(
                    analyst,
                    voteArea,
                    playerControl => playerControl.PlayerId != PlayerControl.LocalPlayer.PlayerId,
                    (playerControl, role) => analyst.GuessCorrectly()
                );
            }
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            if (reason != GameOverReason.HumansByVote && reason != GameOverReason.HumansByTask) return true;

            foreach (Role role in Role.GetRoles(RoleEnum.Analyst))
            {
                Analyst analyst = (Analyst) role;
                analyst.Loses();
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.AnalystLose,
                SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            return true;
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            var role = Role.AllRoles.FirstOrDefault(x =>
                x.RoleType == RoleEnum.Analyst && ((Analyst) x).AnalystWins);
            if (role == null) return;
            if (Role.GetRoles(RoleEnum.Jester).Any(x => ((Jester) x).VotedOut)) return;
            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();
            array[0].NameText.text = role.ColorString + array[0].NameText.text + "</color>";
            __instance.BackgroundBar.material.color = role.Color;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Analyst wins";
            text.color = role.Color;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}