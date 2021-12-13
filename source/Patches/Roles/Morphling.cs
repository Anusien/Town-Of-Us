using System;
using TownOfUs.Extensions;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Morphling : Role, IVisualAlteration

    {
        public KillButtonManager _morphButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastMorphed;
        public PlayerControl MorphedPlayer;

        public PlayerControl SampledPlayer;
        public float TimeRemaining;
        
        public Morphling() { }
        public Morphling(PlayerControl player) : base(player)
        {
        }

        public KillButtonManager MorphButton
        {
            get => _morphButton;
            set
            {
                _morphButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public override string Name => "Transform into crewmates";
        public override Color Color { get; } = Palette.ImpostorRed;
        public override Faction Faction => Faction.Impostors;
        protected override string ImpostorText => "Transform into crewmates";
        protected override string TaskText => "Morph into crewmates to be disguised";
        public override RoleEnum RoleType => RoleEnum.Morphling;

        protected override void DoOnGameStart()
        {
            LastMorphed = DateTime.UtcNow;
        }

        protected override void DoOnMeetingEnd()
        {
            MorphButton.renderer.sprite = TownOfUs.SampleSprite;
            SampledPlayer = null;
            LastMorphed = DateTime.UtcNow;
        }

        public bool Morphed => TimeRemaining > 0f;

        public void Morph()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Morph(Player, MorphedPlayer);
        }

        public void Unmorph()
        {
            MorphedPlayer = null;
            Utils.Unmorph(Player);
            LastMorphed = DateTime.UtcNow;
        }

        public float MorphTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMorphed;
            var num = CustomGameOptions.MorphlingCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (Morphed)
            {
                appearance = MorphedPlayer.GetDefaultAppearance();
                var modifier = Modifier.GetModifier(MorphedPlayer);
                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }
    }
}
