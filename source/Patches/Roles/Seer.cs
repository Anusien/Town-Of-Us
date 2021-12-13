using System;
using System.Collections.Generic;
using TownOfUs.CrewmateRoles.SeerMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Seer : Role
    {
        public readonly Dictionary<byte, bool> Investigated = new Dictionary<byte, bool>();
        
        public Seer() { }
        public Seer(PlayerControl player) : base(player)
        {
            LastInvestigated = DateTime.UtcNow;
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastInvestigated { get; set; }

        public override string Name => "Seer";
        public override Color Color { get; } = new Color(1f, 0.8f, 0.5f, 1f);
        public override Faction Faction => Faction.Crewmates;
        protected override string ImpostorText => "Investigate roles";
        protected override string TaskText => "Investigate roles and find the Impostor";
        public override RoleEnum RoleType => RoleEnum.Seer;

        protected override void DoOnGameStart()
        {
            LastInvestigated = DateTime.UtcNow;
        }

        protected override void DoOnMeetingEnd()
        {
            LastInvestigated = DateTime.UtcNow;
        }

        public float SeerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvestigated;
            var num = CustomGameOptions.SeerCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool CheckSeeReveal(PlayerControl player)
        {
            var role = GetRole(player);
            switch (CustomGameOptions.SeeReveal)
            {
                case SeeReveal.All:
                    return true;
                case SeeReveal.Nobody:
                    return false;
                case SeeReveal.ImpsAndNeut:
                    return role != null && role.Faction != Faction.Crewmates || player.Data.IsImpostor;
                case SeeReveal.Crew:
                    return role != null && role.Faction == Faction.Crewmates || !player.Data.IsImpostor;
            }

            return false;
        }
    }
}
