using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class BigBoiModifier : Modifier, IVisualAlteration
    {
        public static float SpeedFactor = 0.7f;
        
        public BigBoiModifier(PlayerControl player) : base(player)
        {
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor = SpeedFactor;
            appearance.SizeFactor = new Vector3(1.0f, 1.0f, 1.0f);
            return true;
        }

        protected internal override string Name => "Giant";
        protected internal override Color Color { get; } = new Color(1f, 0.5f, 0.5f, 1f);
        protected internal override ModifierEnum ModifierType => ModifierEnum.BigBoi;
        protected internal override string TaskText => "Super slow!";
    }
}