using System.Text.RegularExpressions;

namespace TownOfUs.Extensions
{
    public static class StringExtensions
    {
        public static string StripHtmlTag(this string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
}