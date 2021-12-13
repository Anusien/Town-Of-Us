using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class Flash : Modifier, IVisualAlteration
    {
        public static float SpeedFactor = 1.23f;

        public Flash(PlayerControl player) : base(player)
        {
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor = SpeedFactor;
            return true;
        }

        protected internal override string Name => "Flash";
        protected internal override Color Color { get; } = new Color(1f, 0.5f, 0.5f, 1f);
        protected internal override ModifierEnum ModifierType => ModifierEnum.Flash;
        protected internal override string TaskText => "Superspeed!";
    }
}