using HarmonyLib;

namespace TownOfUs.Roles
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.SetTarget))]
    public class SetTarget
    {
        private static PlayerControl Target;

        /*redifine target for impostor or loving impostor if 1 of this options was activated :
        - anonymous impostors
        - can't kill player in vent
        - loving impostor can't kill his lover*/
        public static bool Prefix(ref PlayerControl target)
        {
            if (Role.GetRole(PlayerControl.LocalPlayer).Faction != Faction.Impostors) return true;
            if ((CustomGameOptions.KillVent ||
                (CustomGameOptions.ImpostorsKnowTeam == AnonymousEnum.AnonymousImpostors ||
                CustomGameOptions.ImpostorsKnowTeam == AnonymousEnum.ImpostorsWinAlone)) ||
                (CustomGameOptions.LoverKill &&
                PlayerControl.LocalPlayer.Is(RoleEnum.LoverImpostor))
            )
                    target = Target;
            return true;
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class Update
        {
            //we redirect the setTarget method to our, we make the verification for the options again
            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;
                if (__instance.KillButton == null) return;
                if (Role.GetRole(PlayerControl.LocalPlayer) == null) return;
                if (((CustomGameOptions.KillVent ||
                    (CustomGameOptions.ImpostorsKnowTeam == AnonymousEnum.AnonymousImpostors ||
                    CustomGameOptions.ImpostorsKnowTeam == AnonymousEnum.ImpostorsWinAlone)) &&
                    Role.GetRole(PlayerControl.LocalPlayer).Faction == Faction.Impostors) ||
                    (CustomGameOptions.LoverKill &&
                    PlayerControl.LocalPlayer.Is(RoleEnum.LoverImpostor)))
                        Utils.SetTarget(ref Target, __instance.KillButton);
            }
        }
    }
}