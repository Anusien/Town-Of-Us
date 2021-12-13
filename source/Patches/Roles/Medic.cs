using UnityEngine;

namespace TownOfUs.Roles
{
    public class Medic : Role
    {
        public Medic() { }
        public Medic(PlayerControl player) : base(player)
        {
            ShieldedPlayer = null;
        }

        public PlayerControl ClosestPlayer;
        public bool UsedAbility { get; set; } = false;
        public PlayerControl ShieldedPlayer { get; set; }
        public PlayerControl exShielded { get; set; }

        public override string Name => "Medic";
        public override Color Color { get; } = new Color(0f, 0.4f, 0f, 1f);
        public override Faction Faction => Faction.Crewmates;
        protected override string ImpostorText => "Create a shield to protect a crewmate";
        protected override string TaskText => "Protect a crewmate with a shield";
        public override RoleEnum RoleType => RoleEnum.Medic;
    }
}
