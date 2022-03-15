using System;
using UnityEngine;
using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Grenadier : RoleWithCooldown
    {
        private KillButton _flashButton;
        public bool Enabled;
        public float TimeRemaining;
        private static Il2CppSystem.Collections.Generic.List<PlayerControl> _closestPlayers;

        static readonly Color normalVision = new Color(0.83f, 0.83f, 0.83f, 0f);
        static readonly Color dimVision = new Color(0.83f, 0.83f, 0.83f, 0.2f);
        static readonly Color blindVision = new Color(0.83f, 0.83f, 0.83f, 1f);

        public Grenadier(PlayerControl player) : base(player, RoleEnum.Grenadier, CustomGameOptions.GrenadeCooldown)
        {
            ImpostorText = () => "Hinder the Crewmates Vision";
            TaskText = () => "Blind the crewmates to get sneaky kills";
        }

        protected override void DoOnGameStart()
        {
            base.DoOnGameStart();
            TimeRemaining = 0f;
        }

        protected override void DoOnMeetingEnd()
        {
            base.DoOnMeetingEnd();
            TimeRemaining = 0f;
        }

        public bool Flashed => TimeRemaining > 0f;


        public KillButton FlashButton
        {
            get => _flashButton;
            set
            {
                _flashButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Flash()
        {
            if (Enabled != true)
            {
                _closestPlayers = FindClosestPlayers(Player);
            }
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            bool sabActive = Utils.IsSabotageActive();

            foreach (var player in _closestPlayers)
            {
                if (PlayerControl.LocalPlayer.PlayerId != player.PlayerId)
                {
                    continue;
                }

                // TODO: Hoist `!sabActive`
                if (TimeRemaining > CustomGameOptions.GrenadeDuration - 0.5f && !sabActive)
                {
                    float fade = (TimeRemaining - CustomGameOptions.GrenadeDuration) * -2.0f;
                        if (ShouldPlayerBeBlinded(player))
                    {
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(normalVision, blindVision, fade);
                    }
                        else if (ShouldPlayerBeDimmed(player))
                    {
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(normalVision, dimVision, fade);
                        if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                        {
                            MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
                        }
                    }
                    else
                    {
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                    }
                }
                else if (TimeRemaining <= (CustomGameOptions.GrenadeDuration - 0.5f) && TimeRemaining >= 0.5f && !sabActive)
                {
                        if (ShouldPlayerBeBlinded(player))
                    {
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = blindVision;
                    }
                        else if (ShouldPlayerBeDimmed(player))
                    {
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = dimVision;
                        if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                        {
                            MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
                        }
                    }
                    else
                    {
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                    }
                }
                else if (TimeRemaining < 0.5f && !sabActive)
                {
                    float fade2 = (TimeRemaining * -2.0f) + 1.0f;
                        if (ShouldPlayerBeBlinded(player))
                    {
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(blindVision, normalVision, fade2);
                    }
                        else if (ShouldPlayerBeDimmed(player))
                    {
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(dimVision, normalVision, fade2);
                        if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                        {
                            MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
                        }
                    }
                    else
                    {
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                    }
                }
                else
                {
                    ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                        DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                    TimeRemaining = 0.0f;
                }
            }

            if (TimeRemaining > 0.5f)
            {
                if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                {
                    MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
                }
            }
        }

        private static bool ShouldPlayerBeDimmed(PlayerControl player) {
            return (player.Data.IsImpostor() || player.Data.IsDead) && !MeetingHud.Instance;
        }

        private static bool ShouldPlayerBeBlinded(PlayerControl player) {
            return !player.Data.IsImpostor() && !player.Data.IsDead && !MeetingHud.Instance;
        }

        public void UnFlash()
        {
            Enabled = false;
            ResetCooldownTimer();
            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
        }

        private static Il2CppSystem.Collections.Generic.List<PlayerControl> FindClosestPlayers(PlayerControl player)
        {
            Il2CppSystem.Collections.Generic.List<PlayerControl> playerControlList = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            float impostorLightMod = PlayerControl.GameOptions.ImpostorLightMod;
            Vector2 truePosition = player.GetTruePosition();
            Il2CppSystem.Collections.Generic.List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
            for (int index = 0; index < allPlayers.Count; ++index)
            {
                GameData.PlayerInfo playerInfo = allPlayers[index];
                if (!playerInfo.Disconnected)
                {
                    Vector2 vector2 = new Vector2(playerInfo.Object.GetTruePosition().x - truePosition.x, playerInfo.Object.GetTruePosition().y - truePosition.y);
                    float magnitude = ((Vector2) vector2).magnitude;
                    if (magnitude <= impostorLightMod * 5)
                    {
                        PlayerControl playerControl = playerInfo.Object;
                        playerControlList.Add(playerControl);
                    }
                }
            }
            return playerControlList;
        }
    }
}