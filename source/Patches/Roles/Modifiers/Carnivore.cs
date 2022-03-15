using System.Collections.Generic;
using HarmonyLib;
using Hazel;
using TownOfUs.ImpostorRoles.CamouflageMod;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class Carnivore : Modifier
    {
        private readonly HashSet<byte> _eaten = new HashSet<byte>();
        public Carnivore(PlayerControl player) : base(player, ModifierEnum.Carnivore)
        {
            Name = "Carnivore";
            TaskText = () => "Kill players to learn their identity.";
            Color = new Color(0.55f, 0.20f, 0.07f);
        }

        // Exists to make it easier to change to a role instead of a modifier
        public static Carnivore Get(PlayerControl player)
        {
            return Modifier.GetModifier<Carnivore>(player);
        }

        // Exists to make it easier to change to a role instead of a modifier
        public static bool IsCarnivore(PlayerControl player)
        {
            return Get(player)?.ModifierType == ModifierEnum.Carnivore;
        }

        public void Eat(byte playerId)
        {
            _eaten.Add(playerId);
        }

        public bool HasEaten(byte playerId)
        {
            return _eaten.Contains(playerId);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public class CarnivoreKill
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (!Carnivore.IsCarnivore(__instance))
            {
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.CarnivoreEat, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            Carnivore carnivore = Carnivore.Get(__instance);
            carnivore.Eat(target.PlayerId);
        }
    }

    // TODO: Refactor with the Coroner to share code
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ShowCarnivorePlayers
    {
        [HarmonyPriority(Priority.Last)]
        private static void Postfix(HudManager __instance)
        {
            if (
                MeetingHud.Instance == null
                || PlayerControl.AllPlayerControls.Count <= 1
                || PlayerControl.LocalPlayer == null
                || PlayerControl.LocalPlayer.Data == null
                || !Carnivore.IsCarnivore(PlayerControl.LocalPlayer)
                || (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeRoles)
            )
            {
                return;
            }

            UpdateMeeting(MeetingHud.Instance);
        }

        private static void UpdateMeeting(MeetingHud __instance)
        {
            Carnivore carnivore = Carnivore.Get(PlayerControl.LocalPlayer);
            foreach (PlayerVoteArea voteArea in __instance.playerStates)
            {
                if (!carnivore.HasEaten(voteArea.TargetPlayerId))
                {
                    continue;
                }

                PlayerControl player = Utils.PlayerById(voteArea.TargetPlayerId);
                Role role = Role.GetRole(player);
                voteArea.NameText.color = role.Color;

                if (CamouflageUnCamouflage.IsCamoed && CustomGameOptions.MeetingColourblind)
                {
                    // TODO: Do we need this?
                    // voteArea.NameText.text = player.name;
                }
                else
                {
                    voteArea.NameText.text = player.name + $" ({role.Name})";
                }
            }
        }
    }
}