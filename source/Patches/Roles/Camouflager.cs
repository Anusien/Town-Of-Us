using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Camouflager : Role

    {
        public KillButtonManager _camouflageButton;
        public bool Enabled;
        public DateTime LastCamouflaged;
        public float TimeRemaining;
        
        public Camouflager() { }
        public Camouflager(PlayerControl player) : base(player)
        { }

        public override string Name => "Camouflager";
        public override Color Color { get; } = Palette.ImpostorRed;
        public override Faction Faction => Faction.Impostors;
        protected override string ImpostorText => "Camouflage and turn everyone grey";
        protected override string TaskText => "Camouflage and get secret kills";
        public override RoleEnum RoleType => RoleEnum.Camouflager;

        protected override void DoOnGameStart()
        {
            LastCamouflaged = DateTime.UtcNow;
        }

        protected override void DoOnMeetingEnd()
        {
            LastCamouflaged = DateTime.UtcNow;
        }

        public bool Camouflaged => TimeRemaining > 0f;

        public KillButtonManager CamouflageButton
        {
            get => _camouflageButton;
            set
            {
                _camouflageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Camouflage()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.Camouflage();
        }

        public void UnCamouflage()
        {
            Enabled = false;
            LastCamouflaged = DateTime.UtcNow;
            Utils.UnCamouflage();
        }

        public float CamouflageTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCamouflaged;
            var num = CustomGameOptions.CamouflagerCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
