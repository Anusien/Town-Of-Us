using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnhollowerBaseLib;
using UnityEngine;

namespace TownOfUs.Handshake
{
    public static class ClientHandshake
    {
        public static bool hideLobbyCode = false;
        //this should really be automated.
        const string auVersion = "2021-12-15";
        //do not change unless you are forking TOU to make your own variation on the mod, this is used to identify different forks of the same mod only.
        const string touForkName = "Anusien";
        public static System.Version touVersion = System.Version.Parse(TownOfUs.GetVersion());

        private static float timer = 600f;
        private static float kickingTimer = 0f;
        private static float kickingTimerMax = 20f;
        private static bool versionSent = false;
        private static string lobbyCodeText = "";

        public static Dictionary<int, PlayerVersionInfo> playerVersions = new Dictionary<int, PlayerVersionInfo>();

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
        public class AmongUsClientOnPlayerJoinedPatch
        {
            public static void Postfix()
            {
                if (PlayerControl.LocalPlayer != null)
                {
                    sendVersion();
                }
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
        public class GameStartManagerStartPatch
        {
            public static void Postfix(GameStartManager __instance)
            {
                // Trigger version refresh
                versionSent = false;
                // Reset lobby countdown timer
                timer = 600f;
                // Reset kicking timer
                kickingTimer = 0f;
                // Copy lobby code
                string code = InnerNet.GameCode.IntToGameName(AmongUsClient.Instance.GameId);
                GUIUtility.systemCopyBuffer = code;
                lobbyCodeText = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.RoomCode, new Il2CppReferenceArray<Il2CppSystem.Object>(0)) + "\r\n" + code;
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        public class GameStartManagerUpdatePatch
        {
            private static bool update = false;
            private static string currentText = "";

            public static void Prefix(GameStartManager __instance)
            {
                if (!AmongUsClient.Instance.AmHost || !GameData.Instance) return; // Not host or no instance
                update = GameData.Instance.PlayerCount != __instance.LastPlayerCount;
            }

            public static void Postfix(GameStartManager __instance)
            {
                // Send version as soon as PlayerControl.LocalPlayer exists
                if (PlayerControl.LocalPlayer != null && !versionSent)
                {
                    versionSent = true;
                    sendVersion();
                }

                // Host update with version handshake infos
                if (AmongUsClient.Instance.AmHost)
                {
                    bool blockStart = false;
                    string message = "";
                    
                    foreach (InnerNet.ClientData client in AmongUsClient.Instance.allClients.ToArray())
                    {
                        if (client.Character == null) continue;
                        var dummyComponent = client.Character.GetComponent<DummyBehaviour>();
                        if (dummyComponent != null && dummyComponent.enabled)
                            continue;
                        else if (!playerVersions.ContainsKey(client.Id))
                        {
                            blockStart = true;
                            Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"ClientHandshake --- {client.Character.Data.PlayerName} has a different or no version of Town Of Us");
                            message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has a different or no version of Town Of Us\n</color>";
                        }
                        else
                        {
                            PlayerVersionInfo playerVI = playerVersions[client.Id];
                            int diff = touVersion.CompareTo(playerVI.touVersion);
                            if (diff > 0)
                            {
                                Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"ClientHandshake --- {client.Character.Data.PlayerName} has an older version of Town Of Us (v{playerVI.touVersion.ToString()})");
                                message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has an older version of Town Of Us (v{playerVI.touVersion.ToString()})\n</color>";
                                blockStart = true;
                            }
                            else if (diff < 0)
                            {
                                Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"ClientHandshake --- {client.Character.Data.PlayerName} has a newer version of Town Of Us (v{playerVI.touVersion.ToString()})");
                                message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has a newer version of Town Of Us (v{playerVI.touVersion.ToString()})\n</color>";
                                blockStart = true;
                            }
                            else if (playerVI.guid!=Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId)
                            { // different guid but same version = modified/different fork/different mod, so shows the fork name.
                                Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"ClientHandshake --- {client.Character.Data.PlayerName} has a modified version of TOU v{playerVI.touVersion.ToString()} - [{playerVI.forkName}]({playerVI.guid.ToString()})");
                                message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has a modified version of TOU v{playerVI.touVersion.ToString()}\n<size=75%>[{playerVI.forkName}]({playerVI.guid.ToString()})</size>\n</color>";
                                blockStart = true;
                            }
                        }
                    }
                    if (blockStart)
                    {
                        __instance.StartButton.color = __instance.startLabelText.color = Palette.DisabledClear;
                        __instance.GameStartText.text = message;
                        __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + Vector3.up * 2;
                    }
                    else
                    {
                        __instance.StartButton.color = __instance.startLabelText.color = ((__instance.LastPlayerCount >= __instance.MinPlayers) ? Palette.EnabledColor : Palette.DisabledClear);
                        __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition;
                    }
                }


                // Client update with handshake infos
                if (!AmongUsClient.Instance.AmHost)
                {
                    string message = "";

                    //for auto update mod coming soon.
                    bool triggerUpdate = false;
                    string hostVersion = "";

                    PlayerVersionInfo hostVI = playerVersions[AmongUsClient.Instance.HostId];
                    int diff = touVersion.CompareTo(hostVI.touVersion);
                    if (!playerVersions.ContainsKey(AmongUsClient.Instance.HostId) || touVersion.CompareTo(hostVI.touVersion) != 0 || Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId != hostVI.guid)
                    {
                        kickingTimer += Time.deltaTime;
                        if (kickingTimer > kickingTimerMax)
                        {
                            kickingTimer = 0;
                            AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
                            SceneChanger.ChangeScene("MainMenu");
                        }
                        if (!playerVersions.ContainsKey(AmongUsClient.Instance.HostId))
                        {
                            Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"ClientHandshake --- The Host({AmongUsClient.Instance.GetHost().PlayerName}) is not a Town Of Us Host");
                            message += $"<color=#FF0000FF>The Host({AmongUsClient.Instance.GetHost().PlayerName}) is not a Town Of Us Host\nYou will be kicked in {Math.Round(10 - kickingTimer)} seconds.</color>";
                        }
                        else if (diff > 0)
                        {
                            Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"ClientHandshake --- The Host({AmongUsClient.Instance.GetHost().PlayerName}) has an older version of Town Of Us (v{hostVI.touVersion.ToString()})");
                            message += $"<color=#FF0000FF>The Host({AmongUsClient.Instance.GetHost().PlayerName}) has an older version of Town Of Us (v{hostVI.touVersion.ToString()})\nYou will be kicked in {Math.Round(kickingTimerMax - kickingTimer)} seconds.</color>";
                            //for linking up with the auto mod update
                            triggerUpdate = true;
                            hostVersion = hostVI.touVersion.ToString();
                        }
                        else if (diff < 0)
                        {
                            Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"ClientHandshake --- The Host({AmongUsClient.Instance.GetHost().PlayerName}) has a newer version of Town Of Us (v{hostVI.touVersion.ToString()})");
                            message += $"<color=#FF0000FF>The Host({AmongUsClient.Instance.GetHost().PlayerName}) has a newer version of Town Of Us (v{hostVI.touVersion.ToString()})\nYou will be kicked in {Math.Round(kickingTimerMax - kickingTimer)} seconds.</color>";
                            //for linking up with the auto mod update
                            triggerUpdate = true;
                            hostVersion = hostVI.touVersion.ToString();
                        }
                        else if (Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId != hostVI.guid)
                        {
                            Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"ClientHandshake --- The Host({AmongUsClient.Instance.GetHost().PlayerName}) has a modified version of TOU v{hostVI.touVersion.ToString()} - [Fork:- {hostVI.forkName}](GUID:-{hostVI.guid.ToString()})");
                            message += $"<color=#FF0000FF>The Host({AmongUsClient.Instance.GetHost().PlayerName}) has a modified version of TOU v{hostVI.touVersion.ToString()}\n<size=75%>[Fork:- {hostVI.forkName}](GUID:-{hostVI.guid.ToString()})</size>\nYou will be kicked in {Math.Round(kickingTimerMax - kickingTimer)} seconds.</color>";
                            triggerUpdate = true;
                            hostVersion = hostVI.touVersion.ToString();
                        }
                        
                        __instance.GameStartText.text = message;
                        __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + Vector3.up * 2;
                        if (triggerUpdate == true && kickingTimer == kickingTimerMax)
                        {
                            //do auto update --- not implemented yet as the auto update system is not implemented yet.
                            //basically.. after lobby disconnect...
                            //trigger an auto update to save the user having to re-open the game...
                            //the version it should be updating to is 'hostversion'
                            //this will require a change to the RPC
                        }
                    }
                    else
                    {
                        __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition;
                        if (__instance.startState != GameStartManager.StartingStates.Countdown)
                        {
                            __instance.GameStartText.text = String.Empty;
                        }
                    }
                }

                // Lobby code replacement
                __instance.GameRoomName.text = hideLobbyCode ? $"<color=#FF0000FF>TownOfUs</color>" : lobbyCodeText;

                // Lobby timer
                if (!AmongUsClient.Instance.AmHost || !GameData.Instance) return; // Not host or no instance

                if (update) currentText = __instance.PlayerCounter.text;

                timer = Mathf.Max(0f, timer -= Time.deltaTime);
                int minutes = (int)timer / 60;
                int seconds = (int)timer % 60;
                string suffix = $" ({minutes:00}:{seconds:00})";

                __instance.PlayerCounter.text = currentText + suffix;
                __instance.PlayerCounter.autoSizeTextContainer = true;
            }
        }

        public static void sendVersion()
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.HostToClientHandshake, Hazel.SendOption.Reliable, -1);
            //write client id = 1 byte read byte
            writer.WritePacked(AmongUsClient.Instance.ClientId);
            //write guid = 16 bytes, read packed int
            writer.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
            //write forkname = flexible length, read string
            writer.Write(touForkName);
            //write tou version.. this should be 5 or 7 bytes long, read string
            writer.Write(TownOfUs.GetVersion());
            //write among us version.. read string for this.
            writer.Write(auVersion);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            logVersion(auVersion, TownOfUs.GetVersion(), Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId, touForkName, AmongUsClient.Instance.ClientId);
        }

        public class PlayerVersionInfo
        {
            public string auVersion { get; set; }
            public System.Version touVersion { get; set; }
            public string forkName { get; set; }
            public Guid guid { get; set; }

            public PlayerVersionInfo(string auVersion, Version touVersion, string forkName, Guid guid)
            {
                this.auVersion = auVersion;
                this.touVersion = touVersion;
                this.forkName = forkName;
                this.guid = guid;
            }
        }

        public static void logVersion(string auVersion, string touVersion, Guid guid, string touFork, int clientId)
        {
            System.Version touVer = new System.Version(touVersion);
            playerVersions[clientId] = new PlayerVersionInfo(auVersion, touVer, touFork, guid);
        }

        public static void readHandshakeViaRPC(MessageReader reader)
        {
            int clientId = reader.ReadPackedInt32();
            Guid guid;
            if (reader.Length - reader.Position >= 16)
            {
                // GUID
                byte[] gbytes = reader.ReadBytes(16);
                guid = new Guid(gbytes);
            }
            else
            {
                guid = new Guid(new byte[16]);
            }
            string forkName = reader.ReadString();
            string touVersion = reader.ReadString();
            string auVersion = reader.ReadString();

            Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"ClientID={clientId}");
            Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"GUID={guid}");
            Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"forkName={forkName}");
            Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"touVersion={touVersion}");
            Reactor.PluginSingleton<TownOfUs>.Instance.Log.LogDebug($"auVersion={auVersion}");
            logVersion(auVersion, touVersion, guid, forkName, clientId);
        }
    }
}
