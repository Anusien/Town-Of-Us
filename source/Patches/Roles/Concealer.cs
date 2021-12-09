using System;
using TownOfUs.ImpostorRoles.CamouflageMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Concealer : RoleWithCooldown
    {
        private KillButtonManager _concealButton;
        public float TimeBeforeConcealed { get; private set; }
        public float ConcealTimeRemaining { get; private set; }
        public PlayerControl Target;
        public PlayerControl Concealed { get; private set; }

        public Concealer(PlayerControl player) : base(player, RoleEnum.Concealer, CustomGameOptions.ConcealCooldown)
        {
            ImpostorText = () => "Conceal crewmates from each other for a sneaky kill";
            TaskText = () => "Conceal crewmates from each other for a sneaky kill";
        }

        protected override void DoOnGameStart()
        {
            base.DoOnGameStart();
            Target = null;
        }

        protected override void DoOnMeetingEnd()
        {
            base.DoOnMeetingEnd();
            Target = null;
        }

        public KillButtonManager ConcealButton
        {
            get => _concealButton;
            set
            {
                _concealButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void StartConceal(PlayerControl concealed)
        {
            Concealed = concealed;
            TimeBeforeConcealed = CustomGameOptions.TimeToConceal;
        }

        /*
         * Called every time incremement. Will manage the time remaining and transition from Waiting to Conceal
         * to Concealed to Unconcealed.
         */
        public void ConcealTick()
        {
            if (Concealed == null)
            {
                return;
            }

            if (TimeBeforeConcealed > 0)
            {
                // Prevent this from going negative to avoid cooldown wonkiness
                TimeBeforeConcealed = Math.Clamp(TimeBeforeConcealed - Time.deltaTime, 0, TimeBeforeConcealed);
                if (TimeBeforeConcealed <= 0f)
                {
                    ConcealTimeRemaining = CustomGameOptions.ConcealDuration;
                }
            }
            else if (ConcealTimeRemaining > 0)
            {
                ConcealTimeRemaining -= Time.deltaTime;
                Conceal();
            }
            else
            {
                Unconceal();
            }
        }

        private void Conceal()
        {
            // If the local player is an impostor, we don't actually want to swoop them
            if (
                PlayerControl.LocalPlayer.Data.IsImpostor
                || PlayerControl.LocalPlayer.Data.IsDead
                || CamouflageUnCamouflage.IsCamoed
                || Concealed == null
                || PlayerControl.LocalPlayer.PlayerId == Concealed.PlayerId
                )
            {
                return;
            }

            Utils.MakeInvisible(Concealed, false);
        }

        private void Unconceal()
        {
            ResetCooldownTimer();
            if (Concealed != null)
            {
                Utils.MakeVisible(Concealed);
                Concealed = null;
            }
        }
    }
}
