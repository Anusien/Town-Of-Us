using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class Torch : Modifier
    {
        public Torch(PlayerControl player) : base(player)
        {
        }

        protected internal override string Name => "Torch";
        protected internal override Color Color { get; } = new Color(1f, 1f, 0.6f);
        protected internal override ModifierEnum ModifierType => ModifierEnum.Torch;
        protected internal override string TaskText => "You can see in the dark.";
    }
}