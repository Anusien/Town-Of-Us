using HarmonyLib;
using TownOfUs.Roles;
using Hazel;

namespace TownOfUs.CrewmateRoles.HaunterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightImpostors
    {
        public static void UpdateMeeting(MeetingHud __instance)
        {
            foreach (var state in __instance.playerStates)
            {
                var role = Role.GetRole(state);
                if (role.Faction == Faction.Impostors || role.RoleType == RoleEnum.Impostor)
                    state.NameText.color = Palette.ImpostorRed;
                if (role.Faction == Faction.Neutral && CustomGameOptions.HaunterRevealsNeutrals)
                    state.NameText.color = role.Color;
            }
        }
        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Haunter)) return;
            var role = Role.GetRole<Haunter>(PlayerControl.LocalPlayer);
            if (!role.CompletedTasks || role.Caught) return;
            if (MeetingHud.Instance)
            {
                UpdateMeeting(MeetingHud.Instance);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                         (byte)CustomRPC.HaunterFinished, SendOption.Reliable, -1);
                writer.Write(role.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}