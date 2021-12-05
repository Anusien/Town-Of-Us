using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TownOfUs.Patches
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class RoleDetailsAttribute : Attribute
    {
        public String Name { get; }
        public String Color { get; }
        public Faction Faction { get; }
        public readonly Color ColorObject;

        public RoleDetailsAttribute(
            string name,
            string color,
            Faction faction
        )
        {
            Name = name;
            Color = color;
            Faction = faction;
            ColorUtility.TryParseHtmlString(Color, out ColorObject);
        }

        public string WrapTextInColor(string text)
        {
            return $"<color={Color}>{text}</color>";
        }

        public string GetColoredName()
        {
            return WrapTextInColor(Name);
        }

        public static RoleDetailsAttribute GetRoleDetails(RoleEnum role)
        {
            MemberInfo memberInfo = typeof(RoleEnum).GetMember(role.ToString()).First();
            RoleDetailsAttribute attribute = memberInfo.GetCustomAttribute<RoleDetailsAttribute>();
            return attribute;

            // TODO: Or this? https://stackoverflow.com/questions/1799370/getting-attributes-of-enums-value
        }
    }
}