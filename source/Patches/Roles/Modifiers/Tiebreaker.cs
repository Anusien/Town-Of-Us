using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class Tiebreaker : Modifier
    {
        public Tiebreaker(PlayerControl player) : base(player)
        {
        }

        protected internal override string Name => "Tiebreaker";
        protected internal override Color Color { get; } = new Color(0.6f, 0.9f, 0.6f);
        protected internal override ModifierEnum ModifierType => ModifierEnum.Tiebreaker;
        protected internal override string TaskText => "Your vote breaks ties";
    }
}