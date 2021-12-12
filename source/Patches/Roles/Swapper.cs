using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Swapper : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();

        public readonly List<bool> ListOfActives = new List<bool>();


        public Swapper(PlayerControl player) : base(player)
        {
        }

        public override string Name => "Swapper";
        public override Color Color { get; } = new Color(0.4f, 0.9f, 0.4f, 1f);
        public override Faction Faction => Faction.Crewmates;
        protected override string ImpostorText => "Swap the votes of two people";
        protected override string TaskText => "Swap two people's votes and wreak havoc!";
        public override RoleEnum RoleType => RoleEnum.Swapper;
    }
}
