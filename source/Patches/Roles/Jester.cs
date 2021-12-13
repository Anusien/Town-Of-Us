using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Jester : Role
    {
        public bool VotedOut;
        
        public Jester() { }
        public Jester(PlayerControl player) : base(player)
        {
        }

        public override string Name => "Jester";
        public override Color Color { get; } = new Color(1f, 0.75f, 0.8f, 1f);
        public override Faction Faction => Faction.Neutral;
        protected override string ImpostorText => "Get voted out";
        protected override string TaskText => "Get voted out!\nFake Tasks:";
        public override RoleEnum RoleType => RoleEnum.Jester;

        protected override void IntroPrefix(IntroCutscene._CoBegin_d__14 __instance)
        {
            var jesterTeam = new List<PlayerControl>();
            jesterTeam.Add(PlayerControl.LocalPlayer);
            __instance.yourTeam = jesterTeam;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (!VotedOut || !Player.Data.IsDead && !Player.Data.Disconnected) return true;
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Jester edition");
            VotedOut = true;
        }

        public void Loses()
        {
            Player.Data.IsImpostor = true;
        }
    }
}
