using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using Reactor.Extensions;
using TownOfUs.CustomOption;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch]
    public static class GameSettings
    {
        public static bool AllOptions;

        /*public static string StringBuild()
        {
            var builder = new StringBuilder("Roles:\n");
            foreach (var option in TownOfUs.Roles)
            {
                builder.AppendLine($"     {option.Name}: {option}");
            }

            builder.AppendLine("Modifiers:");
            foreach (var option in TownOfUs.Modifiers)
            {
                builder.AppendLine($"     {option.Name}: {option}");
            }
            
            
            foreach (var option in TownOfUs.AllOptions)
            {
                builder.AppendLine($"{option.Name}: {option}");
            }
            

            return builder.ToString();
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.FixedUpdate))]
        public static class LobbyFix
        {

            public static bool Prefix()
            {
                
                DestroyableSingleton<HudManager>.Instance.GameSettings.text = StringBuild();
                DestroyableSingleton<HudManager>.Instance.GameSettings.gameObject.SetActive(true);
                return false;
            }
        }


        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyAfter("com.comando.essentials")]
        public static class FixScale
        {
            public static void Prefix(HudManager __instance)
            {
//                __instance.GameSettings.scale = 0.3f;
            }
        }*/

        [HarmonyPatch] //ToHudString
        private static class GameOptionsDataPatch
        {
            public static IEnumerable<MethodBase> TargetMethods()
            {
                return typeof(GameOptionsData).GetMethods(typeof(string), typeof(int));
            }

            private static void Postfix(ref string __result)
            {
                var builder = new StringBuilder(AllOptions ? __result : "");

                foreach (var option in CustomOption.CustomOption.AllOptions)
                {
                    if (option.Name == "Custom Game Settings" && !AllOptions) break;
                    if (option.Type == CustomOptionType.Button) continue;
                    if (option.Type == CustomOptionType.Header) builder.AppendLine($"\n{option.Name}");
                    // no display the title of option for "anonymous impostors"
                    else if (option.Name == "Anon Imp") builder.AppendLine($"{option}\n");
                    else if (option.Indent) builder.AppendLine($"     {option.Name}: {option}");
                    else builder.AppendLine($"{option.Name}: {option}");
                }


                __result = builder.ToString();


                if (CustomOption.CustomOption.LobbyTextScroller && __result.Count(c => c == '\n') > 38)
                    __result = __result.Insert(__result.IndexOf('\n'), " (Scroll for more)");
                else __result = __result.Insert(__result.IndexOf('\n'), "Press Tab to see All Options");


                __result = $"<size=1.25>{__result}</size>";
            }
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.FixedUpdate))]
        private static class LobbyBehaviourPatch
        {
            private static void Postfix()
            {
                if (Input.GetKeyInt(KeyCode.Tab)) AllOptions = !AllOptions;
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        public static class Update
        {
            public static void Postfix(ref GameOptionsMenu __instance)
            {
                /*Automatically calculate the Ybounds.max
                    for the 2021.11.9 the value was (__instance.Children.Length - 8.5f) / 2;*/
                __instance.GetComponentInParent<Scroller>().YBounds.max = (__instance.Children.Length - 7) / 2 + 0.2f;;
            }
        }
    }
}
