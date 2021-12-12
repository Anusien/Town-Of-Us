using System.Collections.Generic;
using TownOfUs.ImpostorRoles.CamouflageMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Snitch : Role
    {
        public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();

        public List<ArrowBehaviour> SnitchArrows = new List<ArrowBehaviour>();

        public List<PlayerControl> SnitchTargets = new List<PlayerControl>();

        public int TasksLeft = int.MaxValue;

        public Snitch(PlayerControl player) : base(player)
        {
            Hidden = !CustomGameOptions.SnitchOnLaunch;
        }

        public bool OneTaskLeft => TasksLeft <= 1;
        public bool TasksDone => TasksLeft <= 0;


        public override string Name => "Snitch";
        public override Color Color { get; } = new Color(0.83f, 0.69f, 0.22f, 1f);
        public override Faction Faction => Faction.Crewmates;
        protected override string ImpostorText => "Complete all your tasks to discover the Impostors";

        protected override string TaskText => TasksDone
            ? "Find the arrows pointing to the Impostors!"
            : "Complete all your tasks to discover the Impostors!";

        public override RoleEnum RoleType => RoleEnum.Snitch;

        internal override bool Criteria()
        {
            return OneTaskLeft && PlayerControl.LocalPlayer.Data.IsImpostor ||
                   base.Criteria();
        }

        protected override string NameText(PlayerVoteArea player = null)
        {
            if (CamouflageUnCamouflage.IsCamoed && player == null) return "";
            if (PlayerControl.LocalPlayer.Data.IsDead) return base.NameText(player);
            if (OneTaskLeft || !Hidden) return base.NameText(player);
            Player.nameText.color = Color.white;
            if (player != null) player.NameText.color = Color.white;
            if (player != null && (MeetingHud.Instance.state == MeetingHud.VoteStates.Proceeding ||
                                   MeetingHud.Instance.state == MeetingHud.VoteStates.Results)) return Player.name;
            if (!CustomGameOptions.RoleUnderName && player == null) return Player.name;
            Player.nameText.transform.localPosition = new Vector3(
                0f,
                Player.Data.HatId == 0U ? 1.5f : 2.0f,
                -0.5f
            );
            return Player.name + "\n" + "Crewmate";
        }
    }
}
