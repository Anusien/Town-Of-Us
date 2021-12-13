using System.Collections.Generic;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Investigator : Role
    {
        public readonly List<Footprint> AllPrints = new List<Footprint>();
        
        public Investigator() { }
        public Investigator(PlayerControl player) : base(player)
        {
        }

        public override string Name => "Investigator";
        public override Color Color { get; } = new Color(0f, 0.7f, 0.7f, 1f);
        public override Faction Faction => Faction.Crewmates;
        protected override string ImpostorText => "Find all imposters by examining footprints";
        protected override string TaskText => "You can see everyone's footprints.";
        public override RoleEnum RoleType => RoleEnum.Investigator;
    }
}
