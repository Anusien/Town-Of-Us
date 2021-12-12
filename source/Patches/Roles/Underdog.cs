using TownOfUs.ImpostorRoles.UnderdogMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Underdog : Role
    {
        public Underdog(PlayerControl player) : base(player)
        {
        }

        public override string Name => "Underdog";
        public override Color Color { get; } = Palette.ImpostorRed;
        public override Faction Faction => Faction.Impostors;
        protected override string ImpostorText => "Use your comeback power to win";
        protected override string TaskText => "You have kill cooldown based on the number of impostors left";
        public override RoleEnum RoleType => RoleEnum.Underdog;

        protected override void DoOnMeetingEnd()
        {
            SetKillTimer();
        }

        public float MaxTimer() => PlayerControl.GameOptions.KillCooldown * (
            PerformKill.LastImp() ? 0.5f : 1.5f
        );

        public void SetKillTimer()
        {
            Player.SetKillTimer(MaxTimer());
        }
    }
}
