using HarmonyLib;
using Reactor;
using TownOfUs.Patches;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Auspex : Role
    {
        public Auspex(PlayerControl player) : base(player, RoleEnum.Auspex)
        {
            ImpostorText = () => "Omens tell you that death has occurred";
            TaskText = () => "Omens tell you that death has occurred";
        }
    }


    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static class FlashForAuspex
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (
                MeetingHud.Instance
                || PlayerControl.LocalPlayer.Data.IsDead
                || !PlayerControl.LocalPlayer.Is(RoleEnum.Altruist)
                || target.PlayerId == PlayerControl.LocalPlayer.PlayerId // Not sure if this is actually needed
            )
            {
                return;
            }

            Color auspexColor = RoleDetailsAttribute.GetRoleDetails(RoleEnum.Auspex).ColorObject;
            Coroutines.Start(Utils.FlashCoroutine(auspexColor));
        }
    }
}