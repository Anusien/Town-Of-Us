using System;
using System.Collections.Generic;
using TownOfUs.CrewmateRoles.SeerMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Seer : Role
    {
        public readonly Dictionary<byte, bool> Investigated = new Dictionary<byte, bool>();

        public Seer(PlayerControl player) : base(player, RoleEnum.Seer)
        {
            ImpostorText = () => "Investigate roles";
            TaskText = () => "Investigate roles and find the Impostor";
            LastInvestigated = DateTime.UtcNow;
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastInvestigated { get; set; }

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
