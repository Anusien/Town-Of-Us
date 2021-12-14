using System;
using System.Collections.Generic;
using HarmonyLib;
using Reactor.Extensions;
using TMPro;
using TownOfUs.ImpostorRoles.AssassinMod;
using TownOfUs.Patches;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public interface IMeetingGuesser
    {
        public Dictionary<byte, (GameObject, GameObject, TMP_Text)> Buttons { get; }

        public Dictionary<byte, int> Guesses { get; }

        public List<RoleEnum> PossibleGuesses { get; }

        public PlayerControl Player { get; }

        public bool CanKeepGuessing();

        private static Sprite CycleSprite => TownOfUs.CycleSprite;

        private static Sprite GuessSprite => TownOfUs.GuessSprite;

        public static void GenButton(
            IMeetingGuesser role,
            PlayerVoteArea voteArea,
            Func<PlayerControl, bool> canGuess,
            Action<PlayerControl, Role> doOnCorrectGuess
            )
        {
            var targetId = voteArea.TargetPlayerId;
            if (IsExempt(voteArea) || !canGuess(Utils.PlayerById(voteArea.TargetPlayerId)))
            {
                role.Buttons[targetId] = (null, null, null);
                return;
            }

            var nameText = Object.Instantiate(voteArea.NameText, voteArea.transform);
            voteArea.NameText.transform.localPosition = new Vector3(0.55f, 0.12f, -0.1f);
            nameText.transform.localPosition = new Vector3(0.55f, -0.12f, -0.1f);
            nameText.text = "Guess";

            var cycle = CreateButton(voteArea, CycleSprite, -0.115f, Cycle(role, voteArea, nameText));
            var guess = CreateButton(voteArea, GuessSprite, 0.115f, Guess(role, voteArea, doOnCorrectGuess));

            role.Guesses.Add(targetId, -1);
            role.Buttons[targetId] = (cycle, guess, nameText);
        }

        private static bool IsExempt(PlayerVoteArea voteArea)
        {
            if (voteArea.AmDead) return true;
            var player = Utils.PlayerById(voteArea.TargetPlayerId);
            if (
                player == null ||
                player.Data.IsDead ||
                player.Data.Disconnected
            )
            {
                return true;
            }

            return false;
        }
        private static GameObject CreateButton(
            PlayerVoteArea voteArea,
            Sprite sprite,
            float yOffset,
            Action onClick
            )
        {
            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var parent = confirmButton.transform.parent.parent;

            var gameObject = Object.Instantiate(confirmButton, voteArea.transform);
            var renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            gameObject.transform.position = confirmButton.transform.position - new Vector3(0.5f, yOffset, 0f);
            gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            gameObject.layer = 5;
            gameObject.transform.parent = parent;

            var button = gameObject.GetComponent<PassiveButton>();

            var clickEvent = button.OnClick = new Button.ButtonClickedEvent();
            clickEvent.AddListener(onClick);
            clickEvent.AddListener((UnityAction)(() =>
            {
                voteArea.Buttons.SetActive(false);
            }));

            var bounds = renderer.bounds;
            bounds.size = new Vector3(0.52f, 0.3f, 0.16f);

            var collider = gameObject.GetComponent<BoxCollider2D>();
            collider.size = renderer.sprite.bounds.size;
            collider.offset = Vector2.zero;
            gameObject.transform.GetChild(0).gameObject.Destroy();

            return gameObject;
        }

        private static Action Cycle(IMeetingGuesser role, PlayerVoteArea voteArea, TextMeshPro nameText)
        {
            void Listener()
            {
                if (MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion) return;

                var currentGuessIdx = role.Guesses[voteArea.TargetPlayerId];
                if (++currentGuessIdx == role.PossibleGuesses.Count)
                    currentGuessIdx = 0;

                var newGuess = role.PossibleGuesses[role.Guesses[voteArea.TargetPlayerId] = currentGuessIdx];

                nameText.text = RoleDetailsAttribute.GetRoleDetails(newGuess).GetColoredName();
            }

            return Listener;
        }

        private static Action Guess(IMeetingGuesser role, PlayerVoteArea voteArea, Action<PlayerControl, Role> doOnCorrectGuess)
        {
            void Listener()
            {
                if (
                    MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion ||
                    IsExempt(voteArea)
                ) return;
                var targetId = voteArea.TargetPlayerId;
                var currentGuessIdx = role.Guesses[targetId];
                if (currentGuessIdx == -1) return;

                RoleEnum currentGuess = role.PossibleGuesses[currentGuessIdx];
                Role actualRole = Role.GetRole(voteArea);

                if (actualRole.RoleType != currentGuess)
                {
                    KillFromMeetingGuess(role, role.Player, actualRole);
                    ShowHideButtons.HideSingle(role, targetId, true);
                    return;
                }

                doOnCorrectGuess(actualRole.Player, actualRole);
                ShowHideButtons.HideSingle(role, targetId, false);
            }

            return Listener;
        }

        public static void KillFromMeetingGuess(IMeetingGuesser guesser, PlayerControl player, Role role)
        {
            AssassinKill.RpcMurderPlayer(player);
            if (player.isLover() && CustomGameOptions.BothLoversDie)
            {
                var lover = ((Lover)role).OtherLover.Player;
                ShowHideButtons.HideSingle(guesser, lover.PlayerId, false);
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    public class ShowHideButtons
    {
        public static void HideButtons(IMeetingGuesser role)
        {
            foreach (var (_, (cycle, guess, guessText)) in role.Buttons)
            {
                if (cycle == null) continue;
                cycle.SetActive(false);
                guess.SetActive(false);
                guessText.gameObject.SetActive(false);

                cycle.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            }
        }

        public static void HideSingle(
            IMeetingGuesser role,
            byte targetId,
            bool killedSelf
        )
        {
            if (
                killedSelf ||
                !role.CanKeepGuessing() ||
                !CustomGameOptions.AssassinMultiKill
            )
            {
                HideButtons(role);
                return;
            }

            var (cycle, guess, guessText) = role.Buttons[targetId];
            if (cycle == null) return;
            cycle.SetActive(false);
            guess.SetActive(false);
            guessText.gameObject.SetActive(false);

            cycle.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            role.Buttons[targetId] = (null, null, null);
            role.Guesses.Remove(targetId);
        }


        public static void Prefix(MeetingHud __instance)
        {
            if (Role.GetRole(PlayerControl.LocalPlayer) is IMeetingGuesser role)
            {
                HideButtons(role);
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))] // BBFDNCCEJHI
    public static class VotingComplete
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (Role.GetRole(PlayerControl.LocalPlayer) is IMeetingGuesser role)
            {
                ShowHideButtons.HideButtons(role);
            }
        }
    }
}
