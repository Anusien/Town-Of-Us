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

        public Prophet(PlayerControl player) : base(player, RoleEnum.Prophet)
        {
            ImpostorText = () => "Finish tasks to find crewmates";
            TaskText = () => "Finish tasks to find crewmates";
        }

        public void Revelation()
        {
            List<PlayerControl> allPlayers = PlayerControl.AllPlayerControls.ToArray().ToList();

            PlayerControl target = allPlayers
                .Where(player => player.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                .Where(player => !Revealed.Contains(player.PlayerId))
                .Where(player => player.Is(Faction.Crewmates))
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