using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Extensions;
using TMPro;
using TownOfUs.ImpostorRoles.CamouflageMod;
using TownOfUs.Patches;
using TownOfUs.Roles.Modifiers;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace TownOfUs.Roles
{
    public abstract class Role
    {
        public static readonly Dictionary<byte, Role> RoleDictionary = new Dictionary<byte, Role>();

        public static bool NobodyWins;

        public readonly List<KillButtonManager> ExtraButtons = new List<KillButtonManager>();

        protected Func<string> ImpostorText;
        protected Func<string> TaskText;

        protected Role(PlayerControl player, RoleEnum roleEnum)
        {
            Player = player;
            RoleDictionary.Add(player.PlayerId, this);
            RoleType = roleEnum;
            RoleDetailsAttribute = RoleDetailsAttribute.GetRoleDetails(roleEnum);
        }

        public static IEnumerable<Role> AllRoles => RoleDictionary.Values.ToList();

        private PlayerControl _player { get; set; }

        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null) _player.nameText.color = Color.white;

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }

        public string Name => RoleDetailsAttribute.Name;
        public Color Color => RoleDetailsAttribute.ColorObject;
        protected internal RoleEnum RoleType { get; }
        private RoleDetailsAttribute RoleDetailsAttribute { get; }

        protected internal bool Hidden { get; set; } = false;

        protected internal Faction Faction => RoleDetailsAttribute.Faction;

        protected internal Color FactionColor
        {
            get
            {
                return Faction switch
                {
                    Faction.Crewmates => Color.green,
                    Faction.Impostors => Color.red,
                    Faction.Neutral => CustomGameOptions.NeutralRed ? Color.red : Color.grey,
                    _ => Color.white
                };
            }
        }

        public string PlayerName { get; private set; }

        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Role other)
        {
            return Equals(Player, other.Player) && RoleType == other.RoleType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Role)) return false;
            return Equals((Role)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)RoleType);
        }

        internal virtual bool Criteria()
        {
            Player.nameText.transform.localPosition = new Vector3(
                0f,
                Player.Data.HatId == 0U ? 1.5f : 2.0f,
                -0.5f
            );
            if (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeRoles) return Utils.ShowDeadBodies;
            if (Faction == Faction.Impostors && PlayerControl.LocalPlayer.Data.IsImpostor &&
                CustomGameOptions.ImpostorsKnowTeam == AnonymousEnum.ImpostorSeeRoles) return true;
            return GetRole(PlayerControl.LocalPlayer) == this;
        }

        /*
         * Hook. Override this method to run before the cutscene showing the player who's on their team.
         * I'm not really sure why anybody does this at the moment.
         */
        protected virtual void IntroPrefix(IntroCutscene._CoBegin_d__14 __instance)
        {
        }

        /*
         * Hook. to simplify creating setting up initial cooldowns and things. Called some time at the start of the
         * game to initialize the player's role.
         * WARNING: This method could be called more than once right now.
         * We don't do these things in the constructor because some constructors will be instantiated more than once.
         * See https://github.com/Anusien/Town-Of-Us/pull/22 for more context.
         */
        protected virtual void DoOnGameStart()
        {
        }

        /*
         * Hook. to simplify resetting cooldowns and things. Called at the end of the meeting to initialize the
         * player's role.
         * See https://github.com/Anusien/Town-Of-Us/pull/22 for more context.
         */
        protected virtual void DoOnMeetingEnd()
        {
        }

        public static void NobodyWinsFunc()
        {
            NobodyWins = true;
        }

        internal static bool NobodyEndCriteria(ShipStatus __instance)
        {
            // TODO
            bool CheckNoImpsNoCrews()
            {
                var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                if (alives.Count == 0) return false;
                var flag = alives.All(x =>
                {
                    var role = GetRole(x);
                    if (role == null) return false;
                    var flag2 = role.Faction == Faction.Neutral && !x.Is(RoleEnum.Glitch) && !x.Is(RoleEnum.Arsonist);
                    var flag3 = x.Is(RoleEnum.Arsonist) && ((Arsonist)role).IgniteUsed && alives.Count > 1;

                    return flag2 || flag3;
                });

                return flag;
            }

            if (CheckNoImpsNoCrews())
            {
                System.Console.WriteLine("NO IMPS NO CREWS");
                var messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.NobodyWins, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(messageWriter);

                NobodyWinsFunc();
                Utils.EndGame();
                return false;
            }

            return true;
        }

        internal virtual bool EABBNOODFGL(ShipStatus __instance)
        {
            return true;
        }

        protected virtual string NameText(PlayerVoteArea player = null)
        {
            if (CamouflageUnCamouflage.IsCamoed && player == null) return "";

            if (Player == null) return "";

            if (player != null && (MeetingHud.Instance.state == MeetingHud.VoteStates.Proceeding ||
                                   MeetingHud.Instance.state == MeetingHud.VoteStates.Results)) return Player.name;

            if (!CustomGameOptions.RoleUnderName && player == null) return Player.name;

            Player.nameText.transform.localPosition = new Vector3(
                0f,
                Player.Data.HatId == 0U ? 1.5f : 2.0f,
                -0.5f
            );
            return Player.name + "\n" + Name;
        }

        public static bool operator ==(Role a, Role b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.RoleType == b.RoleType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Role a, Role b)
        {
            return !(a == b);
        }

        public void RegenTask()
        {
            bool createTask;
            try
            {
                var firstText = Player.myTasks.ToArray()[0].Cast<ImportantTextTask>();
                createTask = !firstText.Text.Contains("Role:");
            }
            catch (InvalidCastException)
            {
                createTask = true;
            }

            if (createTask)
            {
                var task = new GameObject(Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(Player.transform, false);
                task.Text = $"{ColorString}Role: {Name}\n{TaskText()}</color>";
                Player.myTasks.Insert(0, task);
                return;
            }

            Player.myTasks.ToArray()[0].Cast<ImportantTextTask>().Text =
                $"{ColorString}Role: {Name}\n{TaskText()}</color>";
        }

        public static T Gen<T>(Type type, PlayerControl player, CustomRPC rpc)
        {
            var role = (T)Activator.CreateInstance(type, new object[] { player });

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)rpc, SendOption.Reliable, -1);
            writer.Write(player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return role;
        }

        public static T Gen<T>(Type type, List<PlayerControl> players, CustomRPC rpc)
        {
            var player = players[Random.RandomRangeInt(0, players.Count)];
            
            var role = Gen<T>(type, player, rpc);
            players.Remove(player);
            return role;
        }
        
        public static Role GetRole(PlayerControl player)
        {
            if (player == null) return null;
            if (RoleDictionary.TryGetValue(player.PlayerId, out var role))
                return role;

            return null;
        }
        
        public static T GetRole<T>(PlayerControl player) where T : Role
        {
            return GetRole(player) as T;
        }

        public static Role GetRole(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetRole(player);
        }

        public static IEnumerable<Role> GetRoles(RoleEnum roletype)
        {
            return AllRoles.Where(x => x.RoleType == roletype);
        }

        public static class IntroCutScenePatch
        {
            public static TextMeshPro ModifierText;

            [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
            public static class IntroCutscene_BeginCrewmate
            {
                public static void Postfix(IntroCutscene __instance)
                {
                    //System.Console.WriteLine("REACHED HERE - CREW");
                    var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                    if (modifier != null)
                        ModifierText = Object.Instantiate(__instance.Title, __instance.Title.transform.parent, false);
                    //System.Console.WriteLine("MODIFIER TEXT PLEASE WORK");
                    //                        Scale = ModifierText.scale;
                    else
                        ModifierText = null;
                }
            }

            [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
            public static class IntroCutscene_BeginImpostor
            {
                public static void Postfix(IntroCutscene __instance)
                {
                    //System.Console.WriteLine("REACHED HERE - IMP");
                    var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                    if (modifier != null)
                        ModifierText = Object.Instantiate(__instance.Title, __instance.Title.transform.parent, false);
                    //System.Console.WriteLine("MODIFIER TEXT PLEASE WORK");
                    //                        Scale = ModifierText.scale;
                    else
                        ModifierText = null;
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__14), nameof(IntroCutscene._CoBegin_d__14.MoveNext))]
            public static class IntroCutscene_CoBegin__d_MoveNext
            {
                public static void Prefix(IntroCutscene._CoBegin_d__14 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);

                    if (role != null) role.IntroPrefix(__instance);
                }

                public static void Postfix(IntroCutscene._CoBegin_d__14 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);
                    var alpha = __instance.__4__this.Title.color.a;
                    if (role != null && !role.Hidden)
                    {
                        __instance.__4__this.Title.text = role.Name;
                        __instance.__4__this.Title.color = role.Color;
                        __instance.__4__this.ImpostorText.text = role.ImpostorText();
                        __instance.__4__this.ImpostorText.gameObject.SetActive(true);
                        __instance.__4__this.BackgroundBar.material.color = role.Color;
                    }

                    if (ModifierText != null)
                    {
                        var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                        ModifierText.text = "<size=4>Modifier: " + modifier.Name + "</size>";
                        ModifierText.color = modifier.Color;

                        ModifierText.transform.position =
                            __instance.__4__this.transform.position - new Vector3(0f, 2.0f, 0f);
                        ModifierText.gameObject.SetActive(true);
                    }

                    foreach (Role r in AllRoles)
                    {
                        r.DoOnGameStart();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
        public static class PostMeeting
        {
            public static void Postfix()
            {
                foreach (Role r in AllRoles)
                {
                    r.DoOnMeetingEnd();
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__83), nameof(PlayerControl._CoSetTasks_d__83.MoveNext))]
        public static class PlayerControl_SetTasks
        {
            public static void Postfix(PlayerControl._CoSetTasks_d__83 __instance)
            {
                if (__instance == null) return;
                var player = __instance.__4__this;
                var role = GetRole(player);
                var modifier = Modifier.GetModifier(player);

                if (modifier != null)
                {
                    var modTask = new GameObject(modifier.Name + "Task").AddComponent<ImportantTextTask>();
                    modTask.transform.SetParent(player.transform, false);
                    modTask.Text =
                        $"{modifier.ColorString}Modifier: {modifier.Name}\n{modifier.TaskText()}</color>";
                    player.myTasks.Insert(0, modTask);
                }

                if (role == null || role.Hidden) return;
                if (role.RoleType == RoleEnum.Shifter && role.Player != PlayerControl.LocalPlayer) return;
                var task = new GameObject(role.Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text = $"{role.ColorString}Role: {role.Name}\n{role.TaskText()}</color>";
                player.myTasks.Insert(0, task);
            }
        }

        /*[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class ButtonsFix
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (__instance != PlayerControl.LocalPlayer) return;
                var role = GetRole(PlayerControl.LocalPlayer);
                if (role == null) return;
                var instance = DestroyableSingleton<HudManager>.Instance;
                var position = instance.KillButton.transform.position;
                foreach (var button in role.ExtraButtons)
                {
                    button.transform.position = new Vector3(position.x,
                        instance.ReportButton.transform.position.y, position.z);
                }
            }
        }*/

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CheckEndCriteria))]
        public static class ShipStatus_KMPKPPGPNIH
        {
            public static bool Prefix(ShipStatus __instance)
            {
                //System.Console.WriteLine("EABBNOODFGL");
                if (!AmongUsClient.Instance.AmHost) return false;
                if (__instance.Systems.ContainsKey(SystemTypes.LifeSupp))
                {
                    var lifeSuppSystemType = __instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    if (lifeSuppSystemType.Countdown < 0f) return true;
                }

                if (__instance.Systems.ContainsKey(SystemTypes.Laboratory))
                {
                    var reactorSystemType = __instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                    if (reactorSystemType.Countdown < 0f) return true;
                }

                if (__instance.Systems.ContainsKey(SystemTypes.Reactor))
                {
                    var reactorSystemType = __instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();
                    if (reactorSystemType.Countdown < 0f) return true;
                }

                if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks) return true;

                var result = true;
                foreach (var role in AllRoles)
                {
                    //System.Console.WriteLine(role.Name);
                    var isend = role.EABBNOODFGL(__instance);
                    //System.Console.WriteLine(isend);
                    if (!isend) result = false;
                }

                if (!NobodyEndCriteria(__instance)) result = false;

                return result;
                //return true;
            }
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        
        public static class LobbyBehaviour_Start
        {
            private static void Postfix(LobbyBehaviour __instance)
            {
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Snitch))
                {
                    ((Snitch)role).ImpArrows.DestroyAll();
                    ((Snitch)role).SnitchArrows.DestroyAll();
                }

                RoleDictionary.Clear();
                Modifier.ModifierDictionary.Clear();
            }
        }

        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames),
            typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
        public static class TranslationController_GetString
        {
            public static void Postfix(ref string __result, [HarmonyArgument(0)] StringNames name)
            {
                if (ExileController.Instance == null || ExileController.Instance.exiled == null) return;

                //Display role of lover if voted and option activated to prevent other players, anyway when lover die after his lover was voted players know role so wee can show in ejection screen
                var info = ExileController.Instance.exiled;
                var role = GetRole(info.Object);
                if (role == null) return;
                var roleName = "";
                if ((role.RoleType == RoleEnum.Lover || role.RoleType == RoleEnum.LoverImpostor) && CustomGameOptions.VotedLover)
                {
                    var lover = GetRole<Lover>(info.Object);
                    var lover2 = lover.OtherLover.Player;
                    roleName = $"The Lover with {lover2.Data.PlayerName}";
                    __result = $"{info.PlayerName} was {roleName}.";
                    return;
                }

                switch (name)
                {
                    case StringNames.ExileTextPN:
                    case StringNames.ExileTextSN:
                    case StringNames.ExileTextPP:
                    case StringNames.ExileTextSP:
                        {
                            // I've move part of code above
                            roleName = role.RoleType == RoleEnum.Glitch ? role.Name : $"The {role.Name}";
                            __result = $"{info.PlayerName} was {roleName}.";
                            return;
                        }
                }
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManager_Update
        {
            private static Vector3 oldScale = Vector3.zero;
            private static Vector3 oldPosition = Vector3.zero;

            //add the colorType List from DeadBody.cs of MedicMod folder to use here
            public static String GetColorType(PlayerControl player)
            {
                var colors = new Dictionary<int, string>
                {
                    {0, "darker"},// red
                    {1, "darker"},// blue
                    {2, "darker"},// green
                    {3, "lighter"},// pink
                    {4, "lighter"},// orange
                    {5, "lighter"},// yellow
                    {6, "darker"},// black
                    {7, "lighter"},// white
                    {8, "darker"},// purple
                    {9, "darker"},// brown
                    {10, "lighter"},// cyan
                    {11, "lighter"},// lime
                    {12, "darker"},// maroon
                    {13, "lighter"},// rose
                    {14, "lighter"},// banana
                    {15, "darker"},// gray
                    {16, "darker"},// tan
                    {17, "lighter"},// coral
                    {18, "darker"},// watermelon
                    {19, "darker"},// chocolate
                    {20, "lighter"},// sky blue
                    {21, "darker"},// beige
                    {22, "lighter"},// hot pink
                    {23, "lighter"},// turquoise
                    {24, "lighter"},// lilac
                    {25, "darker"},// rainbow
                    {26, "lighter"},// azure
                    {27, "darker"},// Panda
                };

                var colorType = colors[player.Data.ColorId];
                return colorType;
            }

            private static void UpdateMeeting(MeetingHud __instance)
            {
                foreach (var player in __instance.playerStates)
                {
                    var role = GetRole(player);
                    if (role != null && role.Criteria())
                    {
                        player.NameText.color = role.Color;
                        player.NameText.text = role.NameText(player);
                        // if (player.NameText.text.Contains("\n"))
                        // {
                        //     var newScale = Vector3.one * 1.8f;
                        //
                        //     // TODO: scale
                        //     var trueScale = player.NameText.transform.localScale / 2;
                        //
                        //
                        //     if (trueScale != newScale) oldScale = trueScale;
                        //     var newPosition = new Vector3(1.43f, 0.055f, 0f);
                        //
                        //     var truePosition = player.NameText.transform.localPosition;
                        //
                        //     if (newPosition != truePosition) oldPosition = truePosition;
                        //
                        //     player.NameText.transform.localPosition = newPosition;
                        //     player.NameText.transform.localScale = newScale;
                        // }
                        // else
                        // {
                        // if (oldPosition != Vector3.zero) player.NameText.transform.localPosition = oldPosition;
                        // if (oldScale != Vector3.zero) player.NameText.transform.localScale = oldScale;
                        // }
                    }
                    else
                    {
                        //Remove red color for impostors in meeting if anonymous impostor
                        var playerChar = PlayerControl.AllPlayerControls.ToArray()
                        .FirstOrDefault(x => x.PlayerId == player.TargetPlayerId);
                        var localPlayer = PlayerControl.LocalPlayer;
                        if (!localPlayer.Data.IsDead && localPlayer != player && player != null && playerChar.Data != null)
                        {
                            if ((CustomGameOptions.ImpostorsKnowTeam == AnonymousEnum.AnonymousImpostors ||
                                CustomGameOptions.ImpostorsKnowTeam == AnonymousEnum.ImpostorsWinAlone) &&
                                localPlayer.Data.IsImpostor &&
                                playerChar.Data.IsImpostor)
                            {
                                player.NameText.color = Color.white;
                                player.NameText.text = player.name;
                            }

                            //Change the player name color for darker color to the medic in meeting with the GetColorType Method
                            if (localPlayer.Is(RoleEnum.Medic) &&
                                GetColorType(playerChar) == "darker" &&
                                __instance.state != MeetingHud.VoteStates.Proceeding &&
                                __instance.state != MeetingHud.VoteStates.Results)
                                    player.NameText.color = Color.black;
                        }

                        try
                        {
                            player.NameText.text = role.Player.name;
                        }
                        catch
                        {
                        }
                    }
                }
            }

            [HarmonyPriority(Priority.First)]
            private static void Postfix(HudManager __instance)
            {
                if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance);

                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    //flag for anonymous impostors options TODO: rework the flag
                    var flag = (CustomGameOptions.ImpostorsKnowTeam == AnonymousEnum.AnonymousImpostors ||
                        CustomGameOptions.ImpostorsKnowTeam == AnonymousEnum.ImpostorsWinAlone) &&
                        !PlayerControl.LocalPlayer.Data.IsDead &&
                        PlayerControl.LocalPlayer.Data.IsImpostor &&
                        PlayerControl.LocalPlayer != player &&
                        player.Data.IsImpostor;

                    if (player.Data != null && (!(player.Data.IsImpostor && PlayerControl.LocalPlayer.Data.IsImpostor) || flag)) //remove red color for impostors in game
                    {
                        player.nameText.text = player.name;
                        player.nameText.color = Color.white;
                        foreach (var bubble in HudManager.Instance.Chat.chatBubPool.activeChildren)
                            if (flag) bubble.Cast<ChatBubble>().NameText.color = Color.white; //remove red color for impostors in chat
                    }

                    var role = GetRole(player);
                    if (role != null)
                        if (role.Criteria())
                        {
                            player.nameText.color = role.Color;
                            player.nameText.text = role.NameText();
                            continue;
                        }

                    if (player.Data != null && PlayerControl.LocalPlayer.Data.IsImpostor && player.Data.IsImpostor) continue;
                }
            }
        }
    }
}
