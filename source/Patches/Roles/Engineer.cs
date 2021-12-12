using UnityEngine;

namespace TownOfUs.Roles
{
    public class Engineer : Role
    {
        public Engineer(PlayerControl player) : base(player)
        {
        }

        public override string Name => "Engineer";
        public override Color Color { get; } = new Color(1f, 0.65f, 0.04f, 1f);
        public override Faction Faction => Faction.Crewmates;
        protected override string ImpostorText => "Maintain important systems on the ship";
        protected override string TaskText => "Vent and fix a sabotage from anywhere!";
        public override RoleEnum RoleType => RoleEnum.Engineer;

        protected override void DoOnMeetingEnd()
        {
            if (CustomGameOptions.EngineerFixPer == EngineerFixPer.Round)
            {
                UsedThisRound = false;
            }
        }
        public bool UsedThisRound { get; set; } = false;

        public enum EngineerFixPer
        {
            Round,
            Game
        }
    }
}
