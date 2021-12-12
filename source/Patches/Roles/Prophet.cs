using System;
using System.Collections.Generic;
using System.Linq;
using Reactor;
using Reactor.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Prophet : Role
    {
        public readonly ISet<byte> Revealed = new HashSet<byte>();

        public DateTime LastRevealed { get; set; }

        public Prophet(PlayerControl player) : base(player)
        {
            LastRevealed = DateTime.UtcNow; // We shouldn't have to do this, but the revelation is firing before the DoOnGameStart() hits
        }

        public override string Name => "Prophet";
        public override Color Color { get; } = new Color(0.69f, 0.149f, 1f, 1f);
        public override Faction Faction => Faction.Crewmates;
        protected override string ImpostorText => "Survive and find crewmates";
        protected override string TaskText => "Survive to find all the crewmates";
        public override RoleEnum RoleType => RoleEnum.Prophet;

        protected override void DoOnGameStart()
        {
            LastRevealed = DateTime.UtcNow;

            // I think this will trigger a revelation as soon as the HUD hits
            if (CustomGameOptions.ProphetInitialReveal && Revealed.Count == 0)
            {
                LastRevealed = LastRevealed.AddSeconds(CustomGameOptions.ProphetCooldown * -1).AddSeconds(3);
            }
        }

        protected override void DoOnMeetingEnd()
        {
            LastRevealed = DateTime.UtcNow;
        }

        public void Revelation()
        {
            LastRevealed = DateTime.UtcNow;

            List<PlayerControl> allPlayers = PlayerControl.AllPlayerControls.ToArray().ToList();

            PlayerControl target = allPlayers
                .Where(player => player.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                .Where(player => !Revealed.Contains(player.PlayerId))
                .Where(player => GetRole(player).Faction == Faction.Crewmates)
                .Random();

            if (target == null)
            {
                PluginSingleton<TownOfUs>.Instance.Log.LogMessage(
                    $"The Prophet has no more eligible revelations to receive.");
                return;
            }

            PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"The Prophet has received information that {target.name} is a Crewmate role. "
                                                              + $"Their role is {Role.GetRole(target).Name}. They are currently {(target.Data.IsDead ? "dead" : "alive")}.");
            Revealed.Add(target.PlayerId);

            Coroutines.Start(Utils.FlashCoroutine(Color));
        }
    }
}