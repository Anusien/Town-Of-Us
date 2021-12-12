using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Mayor : Role
    {
        public List<byte> ExtraVotes = new List<byte>();

        public Mayor(PlayerControl player) : base(player)
        {
            VoteBank = CustomGameOptions.MayorVoteBank;
        }

        public int VoteBank { get; set; }
        public bool SelfVote { get; set; }

        public bool VotedOnce { get; set; }

        public PlayerVoteArea Abstain { get; set; }

        public bool CanVote => VoteBank > 0 && !SelfVote;

        public override string Name => "Mayor";
        public override Color Color { get; } = new Color(0.44f, 0.31f, 0.66f, 1f);
        public override Faction Faction => Faction.Crewmates;
        protected override string ImpostorText => "Save your votes to double vote";
        protected override string TaskText => "Save your votes to vote multiple times";
        public override RoleEnum RoleType => RoleEnum.Mayor;
    }
}
