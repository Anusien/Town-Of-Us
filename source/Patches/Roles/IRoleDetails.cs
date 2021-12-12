using UnityEngine;

namespace TownOfUs.Roles
{
    public interface IRoleDetails
    {
        string Name { get; }
        Color Color { get; }
        Faction Faction { get; }
    }
}