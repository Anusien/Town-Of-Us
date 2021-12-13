using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Undertaker : Role
    {
        public KillButtonManager _dragDropButton;
        
        public Undertaker() { }
        public Undertaker(PlayerControl player) : base(player)
        {
        }

        public DateTime LastDragged { get; set; }
        public DeadBody CurrentTarget { get; set; }
        public DeadBody CurrentlyDragging { get; set; }

        public override string Name => "Undertaker";
        public override Color Color { get; } = Palette.ImpostorRed;
        public override Faction Faction => Faction.Impostors;
        protected override string ImpostorText => "Drag bodies and hide them";
        protected override string TaskText => "Drag bodies around to hide them from being reported";
        public override RoleEnum RoleType => RoleEnum.Undertaker;

        protected override void DoOnGameStart()
        {
            LastDragged = DateTime.UtcNow;
        }

        protected override void DoOnMeetingEnd()
        {
            DragDropButton.renderer.sprite = TownOfUs.DragSprite;
            CurrentlyDragging = null;
            LastDragged = DateTime.UtcNow;
        }

        public KillButtonManager DragDropButton
        {
            get => _dragDropButton;
            set
            {
                _dragDropButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float DragTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDragged;
            var num = CustomGameOptions.DragCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
