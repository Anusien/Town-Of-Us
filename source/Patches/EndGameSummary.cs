using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using HarmonyLib;
using System.Linq;

namespace TownOfUs.Patches
{
    [HarmonyPatch]
    static class EndGameSummary
    {
        public static class AdditionalGameData
        {
            public static bool updateFlag = false;
            public static List<PlayerInfo> playerData = new List<PlayerInfo>();
            public static List<KeyValuePair<byte, Role>> roleHistory = new List<KeyValuePair<byte, Role>>();
            public static List<KeyValuePair<string, string>> summaryTextLines = new List<KeyValuePair<string, string>>();
            //public static List<Tuple<string, string>> summaryTextLines = new List<Tuple<string, string>>();

            public static void clear()
            {
                playerData.Clear();
                roleHistory.Clear();
                summaryTextLines.Clear();
            }

            internal class PlayerInfo
            {
                public byte PlayerId { get; set; }
                public string PlayerName { get; set; }
                public Faction Faction { get; set; }
                public Role CurrentRole { get; set; }
                public Modifier Modifier { get; set; }
                public int TasksCompleted { get; set; }
                public int TasksTotal { get; set; }
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            //using game state instead of the flag... which works but spams errors so leaving this commented out for now.
            //if(AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
            if (EndGameSummary.AdditionalGameData.updateFlag)
            {
                //EndGameSummary.TrackRoleHistory();
                EndGameSummary.UpdatePlayerInfo();
            }
        }

        public static void TrackRoleHistory()
        {

            //this might work better if disconnections seem to be a problem.. player.name would need changing to player.PlayerName though.
            //foreach (var player in GameData.Instance.AllPlayers)
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var role = Role.GetRole(player);
                if (role != null && (AdditionalGameData.roleHistory.FindAll(x => x.Key == player.PlayerId).Count==0 ||AdditionalGameData.roleHistory.Last(x => x.Key == player.PlayerId).Value != role))
                {
                    AdditionalGameData.roleHistory.Add(KeyValuePair.Create(player.PlayerId, role));
                    Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"-----------RoleChangeStart--------");
                    Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"RC-PlayerID={player.PlayerId}, playername:-{player.name}");
                    Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"RC-tKey={AdditionalGameData.roleHistory.FindLast(x => x.Key == player.PlayerId).Key}");
                    Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"RC-Value={AdditionalGameData.roleHistory.FindLast(x => x.Key == player.PlayerId).Value}");
                    Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"RC-Count={AdditionalGameData.roleHistory.FindAll(x => x.Key == player.PlayerId).Count}");
                    Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"RC-Total={AdditionalGameData.roleHistory.Count}");
                    Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"------------RoleChangeEnd---------");
                }
            }
        }

        public static void UpdatePlayerInfo()
        {
            TrackRoleHistory();
            List<AdditionalGameData.PlayerInfo> pinfolist = new List<AdditionalGameData.PlayerInfo>();

            foreach (GameData.PlayerInfo player in GameData.Instance.AllPlayers)
            {
                AdditionalGameData.PlayerInfo pinfo = new AdditionalGameData.PlayerInfo();
                byte pid = player.PlayerId;
                pinfo.PlayerId = player.PlayerId;
                pinfo.PlayerName = player.PlayerName;
                pinfo.CurrentRole = Role.GetRole(player);
                pinfo.Faction = pinfo.CurrentRole.Faction;
                pinfo.Modifier = Modifier.GetModifier(player);
                pinfo.TasksCompleted = 0;
                pinfo.TasksTotal = 0;

                foreach (var task in player.Tasks)
                {
                    if (task.Complete == true)
                    {
                        pinfo.TasksCompleted++;
                    }
                    pinfo.TasksTotal++;
                }
                pinfolist.Add(pinfo);
            }
            //added this in so when the summary is shown it does not reveal the player join order
            //, which can be used to decipher who is crew (for example on the keys task on polus)
            //the summary now sorts by winner when loaded later, so this just makes winners and losers..
            //...have a random order, but winners always at the top now.
            pinfolist.Shuffle();
            AdditionalGameData.playerData = pinfolist;
            AdditionalGameData.updateFlag = true;
        }

        public static void LoadGameSummary(EndGameManager __instance)
        {
            AdditionalGameData.updateFlag = false;
            var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
            GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f);
            roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

            var roleSummaryText = new StringBuilder();

            roleSummaryText.AppendLine("End-of-game Summary:");

            foreach (var data in AdditionalGameData.playerData)
            {
                var pid = data.PlayerId;
                //this way works 100%.
                /*var won = "   ";
                foreach (WinningPlayerData winner in TempData.winners)
                {
                    Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"WinnerName=#{winner.Name}# vs PlayerName=#{data.PlayerName}#");
                    if (winner.Name == data.PlayerName)
                    {
                        Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"TRUE");
                        won = $"[<color=#FAD934FF>W</color>]";
                    }
                }*/

                //this now works, left in the old code incase future bugs arise as the old code 100% works
                var won = "";
                foreach (WinningPlayerData winner in TempData.winners)
                {
                    Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"WinnerName=#{winner.PlayerName}# vs PlayerName=#{data.PlayerName}#");
                    if (winner.PlayerName == data.PlayerName)
                    {
                        Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"TRUE");
                        won = $"[<color=#FAD934FF>W</color>]";
                        break;
                    }
                    else
                    {
                        Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"FALSE");
                        won = "   ";
                    }
                }

                string nameinfo = data.PlayerName;

                List<string> nonCrewRolesWithTasks = new List<string>();

                nonCrewRolesWithTasks.Add("Phantom");

                string taskInfo = ((data.Faction != Faction.Crewmates) && !nonCrewRolesWithTasks.Contains(data.CurrentRole.Name)) ? "" : $" <color=#FAD934FF>({data.TasksCompleted}/{data.TasksTotal})</color>";

                string roleInfo = "";
                string modifierInfo = "";
                if (data.CurrentRole != null)
                {
                    roleInfo = $"{data.CurrentRole.ColorString}{data.CurrentRole.Name}</color>";
                }
                if (data.Modifier != null)
                {
                    modifierInfo = $"{data.Modifier.ColorString}{data.Modifier.Name}</color> ";
                }

                //role tracking display
                string rhInfo = "";
                int countOfRoles = 0;
                string setNewLine = "";
                
                //sets how many roles show per line in the role history, reduce in case the roles overlap into the victory/defeat message in the middle
                int rolesPerLine = 4;
                Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"------------------------------------------------------------------------------------");
                Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"RoleHistoryLoop-before-totalrolehistorycount:-{AdditionalGameData.roleHistory.Count}");
                Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"------------------------------------------------------------------------------------");

                foreach (var roleHistory in AdditionalGameData.roleHistory)
                {
                    Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"RoleHistoryLoop-PID.:-{pid} / roleHistory.Key:-{roleHistory.Key}");
                    if (roleHistory.Key==pid)
                    {
                        countOfRoles++;
                        setNewLine = "";
                        if((countOfRoles % rolesPerLine)==0)
                        {
                            setNewLine = $"\n      ";
                        }
                        Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"RoleHistoryLoop-CountOFRoles.:-{countOfRoles} / setnewline:-{setNewLine}");
                        Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"RoleHistoryLoop-CountOFRoles % rolesperline.:-{(countOfRoles % rolesPerLine)}");
                        Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"EGS-PlayerName:-{data.PlayerName} --- playerID:-{pid}(also {roleHistory.Key}) --- RoleName:-{roleHistory.Value.Name}");
                        rhInfo += $"{roleHistory.Value.ColorString}{roleHistory.Value.Name}</color>{setNewLine} > ";
                    }
                }

                Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"rhinfo=#{rhInfo}#");
                Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"rhinfolength={rhInfo.Length}"); 
                Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"currentrolelength={data.CurrentRole.Name.Length}");
                Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"combinedString={String.Concat(data.CurrentRole.ColorString, data.CurrentRole.Name + "</color>").Length + 3 + setNewLine.Length}");

                if (rhInfo.Length > String.Concat(data.CurrentRole.ColorString,data.CurrentRole.Name+"</color>").Length+3+setNewLine.Length)
                {
                    rhInfo = rhInfo.Substring(0, rhInfo.Length - (3+setNewLine.Length));
                    AdditionalGameData.summaryTextLines.Add(KeyValuePair.Create(won, $"<size=80%>{won}{nameinfo} the {modifierInfo}{roleInfo}{taskInfo}</size>\n<size=60%>      {rhInfo}</size><size=40%>\n </size>"));
                    //AdditionalGameData.summaryTextLines.Add(Tuple.Create(won, $"<size=80%>{won}{nameinfo} the {modifierInfo}{roleInfo}{taskInfo}</size>\n<size=60%>      {rhInfo}</size><size=40%>\n </size>"));
                    //roleSummaryText.AppendLine($"<size=80%>{won}{nameinfo} the {modifierInfo}{roleInfo}{taskInfo}</size>\n<size=10%>\n </size><size=60%>{rhinfo}</size><size=40%>\n </size>");
                }
                else
                {
                    AdditionalGameData.summaryTextLines.Add(KeyValuePair.Create(won, $"<size=80%>{won}{nameinfo} the {modifierInfo}{roleInfo}{taskInfo}</size>"));
                    //AdditionalGameData.summaryTextLines.Add(Tuple.Create(won, $"<size=80%>{won}{nameinfo} the {modifierInfo}{roleInfo}{taskInfo}</size>"));
                    //roleSummaryText.AppendLine($"{won}{nameinfo} the {modifierInfo}{roleInfo}{taskInfo}");
                }
            }


            //sorting winners to the top, still in a randomish order.
            //AdditionalGameData.summaryTextLines = AdditionalGameData.summaryTextLines.OrderByDescending(tuple => tuple.Item1).ToList();
            AdditionalGameData.summaryTextLines = AdditionalGameData.summaryTextLines.OrderByDescending(kvp => kvp.Key).ToList();
            Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"SummaryTextLines.Count:-{AdditionalGameData.summaryTextLines.Count}");
            //adding to summary text
            foreach (KeyValuePair<string, string> line in AdditionalGameData.summaryTextLines)
            //foreach (Tuple<string, string> line in AdditionalGameData.summaryTextLines)
            {
                roleSummaryText.AppendLine(line.Value);
                //roleSummaryText.AppendLine(line.Item2);
            }

            TMPro.TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMPro.TMP_Text>();
            roleSummaryTextMesh.alignment = TMPro.TextAlignmentOptions.TopLeft;
            roleSummaryTextMesh.color = Color.white;
            roleSummaryTextMesh.fontSizeMin = 1.5f;
            roleSummaryTextMesh.fontSizeMax = 1.5f;
            roleSummaryTextMesh.fontSize = 1.5f;

            var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
            roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
            roleSummaryTextMesh.text = roleSummaryText.ToString();
        }
    }
}
