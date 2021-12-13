using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class ButtonBarry : Modifier
    {
        public KillButtonManager ButtonButton;

        public bool ButtonUsed;

        public ButtonBarry(PlayerControl player) : base(player)
        {
        }

        protected internal override string Name => "Button Barry";
        protected internal override Color Color { get; } = new Color(0.9f, 0f, 1f, 1f);
        protected internal override ModifierEnum ModifierType => ModifierEnum.ButtonBarry;
        protected internal override string TaskText => "Call a button from anywhere!";
    }
}