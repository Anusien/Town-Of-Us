using UnityEngine;

namespace TownOfUs.Roles
{
    public class Altruist : Role
    {
        public bool CurrentlyReviving;
        public DeadBody CurrentTarget;

        public bool ReviveUsed;
        
        public Altruist() { }
        public Altruist(PlayerControl player) : base(player)
        {
        }
        
        public override string Name => "Altruist";
        public override Color Color { get; } = new Color(0.4f, 0f, 0f, 1f);
        public override Faction Faction => Faction.Crewmates;
        public override RoleEnum RoleType => RoleEnum.Altruist;
        protected override string ImpostorText => "Sacrifice yourself to save another";
        protected override string TaskText => "Revive a dead body at the cost of your own life.";
    }
}
