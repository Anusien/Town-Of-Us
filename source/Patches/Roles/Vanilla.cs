using UnityEngine;

namespace TownOfUs.Roles
{
    public class Impostor : Role
    {
        public Impostor(PlayerControl player) : base(player, RoleEnum.Impostor)
        {
            Hidden = true;
        }
    }

    public class Crewmate : Role
    {
        public Crewmate(PlayerControl player) : base(player, RoleEnum.Crewmate)
        {
            Hidden = true;
        }
    }
}
