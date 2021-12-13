using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class Drunk : Modifier
    {
        public Drunk(PlayerControl player) : base(player)
        {
        }

        protected internal override string Name => "Drunk";
        protected internal override Color Color { get; } = new Color(0.46f, 0.5f, 0f, 1f);
        protected internal override ModifierEnum ModifierType => ModifierEnum.Drunk;
        protected internal override string TaskText => "Inverrrrrted contrrrrols";
    }
}