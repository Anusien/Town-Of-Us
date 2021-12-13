using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Shifter : Role
    {
        public Shifter() { }
        public Shifter(PlayerControl player) : base(player)
        {
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastShifted { get; set; }

        public override string Name => "Shifter";
        public override Color Color { get; } = new Color(0.6f, 0.6f, 0.6f, 1f);
        public override Faction Faction => Faction.Neutral;
        protected override string ImpostorText => "Shift around different roles";
        protected override string TaskText => "Steal other people's roles.\nFake Tasks:";
        public override RoleEnum RoleType => RoleEnum.Shifter;

        protected override void DoOnGameStart()
        {
            LastShifted = DateTime.UtcNow;
        }

        protected override void DoOnMeetingEnd()
        {
            LastShifted = DateTime.UtcNow;
        }

        public void Loses()
        {
            Player.Data.IsImpostor = true;
        }

        public float ShifterShiftTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastShifted;
            var num = CustomGameOptions.ShifterCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
