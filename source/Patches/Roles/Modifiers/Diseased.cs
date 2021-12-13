using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class Diseased : Modifier
    {
        public Diseased(PlayerControl player) : base(player)
        {
        }

        protected internal override string Name => "Diseased";
        protected internal override Color Color { get; } = Color.grey;
        protected internal override ModifierEnum ModifierType => ModifierEnum.Diseased;
        protected internal override string TaskText => "Killing you gives Impostors a high cooldown";
    }
}