﻿using UnityEngine;

namespace TownOfUs.Roles
{
    public class Engineer : Role
    {
        public Engineer(PlayerControl player) : base(player)
        {
            ImpostorText = () => "Maintain important systems on the ship";
            TaskText = () => "Vent and fix a sabotage from anywhere!";
            RoleType = RoleEnum.Engineer;
        }

        protected override void DoOnMeetingEnd()
        {
            if (CustomGameOptions.EngineerFixPer == EngineerFixPer.Round)
            {
                UsedThisRound = false;
            }
        }
        public bool UsedThisRound { get; set; } = false;

        public enum EngineerFixPer
        {
            Round,
            Game
        }
    }
}
