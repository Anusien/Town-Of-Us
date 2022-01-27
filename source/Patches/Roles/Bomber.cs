using Reactor;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    // TODO: Set target for the bomber
    // TODO: Set target for the bombed player
    // TODO: Set cooldowns for the bomber
    // TODO: Set a cooldown for the bombed player?
    public class Bomber : RoleWithCooldown
    {
        private KillButton _plantBombButton;
        private KillButton _killWithBombButton;
        public PlayerControl Target;
        public PlayerControl BombedPlayer;
        public PlayerControl BombedPlayerTarget;
        private float _timeUntilBombArmed;
        private bool _bombArmed;
        private float _timeUntilExplosion;

        public Bomber(PlayerControl player) : base(player, RoleEnum.Bomber, CustomGameOptions.BomberCooldown)
        {
            ImpostorText = () => "Wire crewmates to explode";
            TaskText = () => "Wire crewmates to explode";
        }

        protected override void DoOnMeetingEnd()
        {
            base.DoOnMeetingEnd();
            ClearBomb();
        }

        public KillButton PlantBombButton
        {
            get => _plantBombButton;
            set
            {
                _plantBombButton = value;
                /* TODO
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
                */
            }
        }

        public KillButton KillWithBombButton
        {
            get => _killWithBombButton;
            set
            {
                _killWithBombButton = value;
                /* TODO
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
                */
            }
        }

        /*
         * Encapsulates all the Bomber lifecycle stuff. Pass in a player and this will indicate whether that player
         * is bombed and should get a kill button.
         */
        public bool ShouldShowKillWithBombButton(PlayerControl player)
        {
            return BombedPlayer?.PlayerId == player.PlayerId && _bombArmed;
        }

        public void BombTick()
        {
            if (BombedPlayer == null)
            {
                return;
            }

            if (_bombArmed)
            {
                if (_timeUntilExplosion > 0)
                {
                    _timeUntilExplosion -= Time.deltaTime;
                }
                else
                {
                    // Bomb blows up in their hands; they kill themselves
                    if (BombedPlayer.isShielded())
                    {
                        Utils.BreakShield(BombedPlayer);
                    }
                    else
                    {
                        BombedPlayer.Data.SetImpostor(true);
                        BombedPlayer.MurderPlayer(BombedPlayer);
                        BombedPlayer.Data.SetImpostor(false);
                    }
                }
            } else if (_timeUntilBombArmed <= 0)
            {
                _bombArmed = true;
                _timeUntilExplosion = CustomGameOptions.BombFuseTime;
                Coroutines.Start(Utils.FlashCoroutine(Palette.ImpostorRed));
            }
        }

        public void BombKill(PlayerControl target)
        {
            Utils.RpcMurderPlayer(BombedPlayer, target);
            ClearBomb();
        }

        private void ClearBomb()
        {
            BombedPlayer = null;
            _bombArmed = false;
            _timeUntilBombArmed = 0;
            _timeUntilExplosion = 0;
            ResetCooldownTimer();
        }

        public void PlantBomb(PlayerControl target)
        {
            target.RemainingEmergencies = 0; // TODO: Is there a better way to do this?
            BombedPlayer = target;
            Target = null;
            _timeUntilBombArmed = CustomGameOptions.BombFuseTime;
            _bombArmed = false;
        }
    }
}