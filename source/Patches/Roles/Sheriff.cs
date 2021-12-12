using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Sheriff : Role
    {
        public Sheriff(PlayerControl player) : base(player)
        {
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastKilled { get; set; }

        protected override void DoOnGameStart()
        {
            LastKilled = DateTime.UtcNow;
        }

        protected override void DoOnMeetingEnd()
        {
            LastKilled = DateTime.UtcNow;
        }

        public float SheriffKillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.SheriffKillCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public override string Name => "Sheriff";
        public override Color Color { get; } = Color.yellow;
        public override Faction Faction => Faction.Crewmates;
        protected override string ImpostorText => "Shoot the <color=#FF0000FF>Impostor</color>";
        protected override string TaskText => "Kill off the impostor but don't kill crewmates.";
        public override RoleEnum RoleType => RoleEnum.Sheriff;

        internal override bool Criteria()
        {
            return CustomGameOptions.ShowSheriff || base.Criteria();
        }
    }
}
