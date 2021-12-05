using System;
using TownOfUs.CrewmateRoles.TimeLordMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class TimeLord : Role
    {
        public TimeLord(PlayerControl player) : base(player, RoleEnum.TimeLord)
        {
            ImpostorText = () => "Rewind Time";
            TaskText = () => "Rewind Time!";
        }

        public DateTime StartRewind { get; set; }
        public DateTime FinishRewind { get; set; }

        protected override void DoOnGameStart()
        {
            FinishRewind = DateTime.UtcNow;
            StartRewind = DateTime.UtcNow;
        }

        protected override void DoOnMeetingEnd()
        {
            /*
             * TODO: I don't fully understand why these add -10. In other places I've removed it, but since it has
             * the StartRewind as well, I'm inclined to leave it for now so I don't break Time Lord.
             */
            FinishRewind = DateTime.UtcNow.AddSeconds(-10);
            StartRewind = DateTime.UtcNow.AddSeconds(-20);
        }

        public float TimeLordRewindTimer()
        {
            var utcNow = DateTime.UtcNow;


            TimeSpan timespan;
            float num;

            if (RecordRewind.rewinding)
            {
                timespan = utcNow - StartRewind;
                num = CustomGameOptions.RewindDuration * 1000f / 3f;
            }
            else
            {
                timespan = utcNow - FinishRewind;
                num = CustomGameOptions.RewindCooldown * 1000f;
            }


            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timespan.TotalMilliseconds) / 1000f;
        }


        public float GetCooldown()
        {
            return RecordRewind.rewinding ? CustomGameOptions.RewindDuration : CustomGameOptions.RewindCooldown;
        }
    }
}
