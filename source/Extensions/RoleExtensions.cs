using Reactor.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.Extensions
{
    public static class RoleExtensions
    {
        public static string WrapTextInColor(this IRoleDetails roleDetails, string text)
        {
            return $"<color=#{roleDetails.Color.ToHtmlStringRGBA()}>{text}</color>";
        }

        public static string GetColoredName(this IRoleDetails roleDetails)
        {
            return roleDetails.WrapTextInColor(roleDetails.Name);
        }
    }
}