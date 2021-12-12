using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Executioner : Role
    {
        public PlayerControl target;
        public bool TargetVotedOut;

        public Executioner(PlayerControl player) : base(player)
        {
        }

        public override string Name => "Executioner";
        public override Color Color { get; } = new Color(0.55f, 0.25f, 0.02f, 1f);
        public override Faction Faction => Faction.Neutral;
        protected override string ImpostorText => !target ? "You don't have a target for some reason... weird..." : $"Vote {target.name} out";
        protected override string TaskText => !target ? "You don't have a target for some reason... weird..." : $"Vote {target.name} out\nFake Tasks:";
        public override RoleEnum RoleType { get; }

        protected override void IntroPrefix(IntroCutscene._CoBegin_d__14 __instance)
        {
            var executionerteam = new List<PlayerControl>();
            executionerteam.Add(PlayerControl.LocalPlayer);
            __instance.yourTeam = executionerteam;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead) return true;
            if (!TargetVotedOut || !target.Data.IsDead) return true;
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;
            TargetVotedOut = true;
        }

        public void Loses()
        {
            Player.Data.IsImpostor = true;
        }
    }
}
