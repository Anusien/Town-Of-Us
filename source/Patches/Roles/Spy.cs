using UnityEngine;

namespace TownOfUs.Roles
{
    public class Spy : Role
    {
        public Spy(PlayerControl player) : base(player)
        {
        }

        public override string Name => "Spy";
        public override Color Color { get; } = new Color(0.8f, 0.64f, 0.8f, 1f);
        public override Faction Faction => Faction.Crewmates;
        protected override string ImpostorText => "Snoop around and find stuff out";
        protected override string TaskText => "Spy on people and find the Impostors";
        public override RoleEnum RoleType => RoleEnum.Spy;
    }
}
