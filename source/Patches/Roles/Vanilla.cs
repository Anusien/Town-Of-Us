using UnityEngine;

namespace TownOfUs.Roles
{
    public class Impostor : Role
    {
        public Impostor() { }
        public Impostor(PlayerControl player) : base(player) { }

        public override string Name => "Impostor";
        public override Color Color { get; } = Palette.ImpostorRed;
        public override Faction Faction => Faction.Impostors;
        protected override string ImpostorText => null;
        protected override string TaskText => null;
        public override RoleEnum RoleType => RoleEnum.Impostor;
        protected override bool Hidden => true;
    }

    public class Crewmate : Role
    {
        public Crewmate() { }
        public Crewmate(PlayerControl player) : base(player)
        {
        }

        public override string Name => "Crewmate";
        public override Color Color { get; } = Palette.CrewmateBlue;
        public override Faction Faction => Faction.Crewmates;
        protected override string ImpostorText => null;
        protected override string TaskText => null;
        public override RoleEnum RoleType => RoleEnum.Crewmate;
        protected override bool Hidden => true;
    }
}
