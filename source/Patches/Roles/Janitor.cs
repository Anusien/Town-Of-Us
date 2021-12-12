using UnityEngine;

namespace TownOfUs.Roles
{
    public class Janitor : Role
    {
        public KillButtonManager _cleanButton;

        public Janitor(PlayerControl player) : base(player)
        {
        }

        public DeadBody CurrentTarget { get; set; }

        public KillButtonManager CleanButton
        {
            get => _cleanButton;
            set
            {
                _cleanButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public override string Name => "Janitor";
        public override Color Color { get; } = Palette.ImpostorRed;
        public override Faction Faction => Faction.Impostors;
        protected override string ImpostorText => "Clean up bodies";
        protected override string TaskText => "Clean bodies to prevent Crewmates from discovering them.";
        public override RoleEnum RoleType => RoleEnum.Janitor;
    }
}
