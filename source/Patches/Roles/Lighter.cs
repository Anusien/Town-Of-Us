using UnityEngine;

namespace TownOfUs.Roles
{
    public class Lighter : RoleWithCooldown
    {
        private KillButton _lighterButton;
        public float LighterTimeRemaining;
        public bool IsLighting { get; private set; }

        public Lighter(PlayerControl player) : base(player, RoleEnum.Lighter, CustomGameOptions.LighterCooldown)
        {
            ImpostorText = () => "Need a light?";
            TaskText = () => "Use your lighter for extra visibility.";
        }

        protected override void DoOnGameStart()
        {
            base.DoOnGameStart();
            LighterTimeRemaining = 0f;
        }

        protected override void DoOnMeetingEnd()
        {
            base.DoOnMeetingEnd();
            LighterTimeRemaining = 0f;
        }

        public KillButton LighterButton
        {
            get => _lighterButton;
            set
            {
                _lighterButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void LightTick()
        {
            if (!IsLighting)
            {
                return;
            }

            if (LighterTimeRemaining > 0f)
            {
                LighterTimeRemaining -= Time.deltaTime;
            }
            else
            {
                LightOff();
            }
        }

        public void LightOn()
        {
            IsLighting = true;
            LighterTimeRemaining = CustomGameOptions.LighterDuration;
        }

        private void LightOff()
        {
            IsLighting = false;
            ResetCooldownTimer();
        }
    }
}
