using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Assassin : Role, IMeetingGuesser
    {
        public Dictionary<byte, (GameObject, GameObject, TMP_Text)> Buttons { get; } = new Dictionary<byte, (GameObject, GameObject, TMP_Text)>();
        public Dictionary<byte, int> Guesses { get; } = new Dictionary<byte, int>();
        public List<RoleEnum> PossibleGuesses { get; }
        public Assassin(PlayerControl player) : base(player, RoleEnum.Assassin)
        {
            ImpostorText = () => "Kill during meetings if you can guess their roles";
            TaskText = () => "Guess the roles of the people and kill them mid-meeting";

            RemainingKills = CustomGameOptions.AssassinKills;

            PossibleGuesses = CustomGameOptions.AssassinGuessNeutrals
                ? CustomGameOptions.GetEnabledRoles(Faction.Crewmates, Faction.Neutral)
                : CustomGameOptions.GetEnabledRoles(Faction.Crewmates);

            if (CustomGameOptions.AssassinCrewmateGuess)
                PossibleGuesses.Add(RoleEnum.Crewmate);
            
            // add impostor roles to possible guess if impostor can win alone was activated
            if (CustomGameOptions.ImpostorsKnowTeam == AnonymousEnum.ImpostorsWinAlone)
                PossibleGuesses.AddRange(CustomGameOptions.GetEnabledRoles(Faction.Impostors));
        }

        public int RemainingKills { get; set; }

        public bool CanKeepGuessing() => RemainingKills > 0;
    }
}
