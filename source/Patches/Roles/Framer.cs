using System;
using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Framer : Role
    {
        public Framer(PlayerControl player) : base(player)
        {
            Name = "Framer";
            ImpostorText = () => "Bury your kills in a shallow grave";
            TaskText = () => "Have your kills appear to frame Crewmates.";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Framer;
            Faction = Faction.Impostors;
        }

        public PlayerControl ClosestPlayer;

        public readonly Queue<QueueEntry> BodiesWaiting = new Queue<QueueEntry>();

        public class QueueEntry
        {
            public DeadBody DeadBody { get; }
            public DateTime Deadline { get; }

            public QueueEntry(DeadBody deadBody, DateTime deadline)
            {
                DeadBody = deadBody;
                Deadline = deadline;
            }
        }
    }
}