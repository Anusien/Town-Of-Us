using HarmonyLib;
using Hazel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reactor.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.ImpostorRoles.CamouflageMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using PerformKill = TownOfUs.ImpostorRoles.UnderdogMod.PerformKill;

namespace TownOfUs
{
    [HarmonyPatch]
    public static class Utils
    {
        internal static bool ShowDeadBodies = false;

        public static Dictionary<PlayerControl, Color> oldColors = new Dictionary<PlayerControl, Color>();

        public static List<WinningPlayerData> potentialWinners = new List<WinningPlayerData>();

        public static void MakeInvisible(PlayerControl player, bool showBlur)
        {
            Color color = Color.clear;
            if (showBlur)
            {
                color.a = 0.1f;
            }

            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
            {
                player.SetOutfit(CustomPlayerOutfitType.Swooper, new GameData.PlayerOutfit()
                {
                    ColorId = player.CurrentOutfit.ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    _playerName = " "
                });
                player.MyRend.color = color;
            }
        }

        public static void Morph(PlayerControl player, PlayerControl morphedPlayer, bool resetAnim = false)
        {
            if (CamouflageUnCamouflage.IsCamoed) return;

            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                player.SetOutfit(CustomPlayerOutfitType.Morph, morphedPlayer.Data.DefaultOutfit);
        }

        public static void MakeVisible(PlayerControl player)
        {
            Unmorph(player);
            player.MyRend.color = Color.white;
        }

        public static void Unmorph(PlayerControl player)
        {
            player.SetOutfit(CustomPlayerOutfitType.Default);
        }

        public static void Camouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage)
                {
                    player.SetOutfit(CustomPlayerOutfitType.Camouflage, new GameData.PlayerOutfit()
                    {
                        ColorId = player.GetDefaultOutfit().ColorId,
                        HatId = "",
                        SkinId = "",
                        VisorId = "",
                        _playerName = " "
                    });
                    //player.nameText.text = "";
                    PlayerControl.SetPlayerMaterialColors(Color.grey, player.myRend);
                }
            }
        }

        public static void UnCamouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls) Unmorph(player);
        }

        public static bool IsCrewmate(this PlayerControl player)
        {
            return GetRole(player) == RoleEnum.Crewmate;
        }

        public static void AddUnique<T>(this Il2CppSystem.Collections.Generic.List<T> self, T item)
            where T : IDisconnectHandler
        {
            if (!self.Contains(item)) self.Add(item);
        }

        public static bool isLover(this PlayerControl player)
        {
            return player.Is(RoleEnum.Lover) || player.Is(RoleEnum.LoverImpostor);
        }

        public static bool Is(this PlayerControl player, RoleEnum roleType)
        {
            return Role.GetRole(player)?.RoleType == roleType;
        }

        public static bool Is(this PlayerControl player, ModifierEnum modifierType)
        {
            return Modifier.GetModifier(player)?.ModifierType == modifierType;
        }

        public static bool Is(this PlayerControl player, Faction faction)
        {
            return Role.GetRole(player)?.Faction == faction;
        }

        public static List<PlayerControl> GetCrewmates(List<PlayerControl> impostors)
        {
            return PlayerControl.AllPlayerControls.ToArray().Where(
                player => !impostors.Any(imp => imp.PlayerId == player.PlayerId)
            ).ToList();
        }

        public static List<PlayerControl> GetImpostors(
            List<GameData.PlayerInfo> infected)
        {
            var impostors = new List<PlayerControl>();
            foreach (var impData in infected)
                impostors.Add(impData.Object);

            return impostors;
        }

        public static RoleEnum GetRole(PlayerControl player)
        {
            if (player == null) return RoleEnum.None;
            if (player.Data == null) return RoleEnum.None;

            var role = Role.GetRole(player);
            if (role != null) return role.RoleType;

            return player.Data.IsImpostor() ? RoleEnum.Impostor : RoleEnum.Crewmate;
        }

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == id)
                    return player;

            return null;
        }

        public static bool isShielded(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).Any(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            });
        }

        public static Medic getMedic(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).FirstOrDefault(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            }) as Medic;
        }

        /*
         * TODO
         * Can we make a clean encapsulation of this that checks for shield, breaks it, and also resets cooldowns
         * if the setting is on? That would be another step toward reducing boilerplate and making new roles easier.
         */
        public static void BreakShield(PlayerControl target)
        {
            if (!target.isShielded())
            {
                return;
            }

            byte medicIc = target.getMedic().Player.PlayerId;
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.AttemptSound, SendOption.Reliable, -1);
            writer.Write(medicIc);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            StopKill.BreakShield(medicIc, target.PlayerId, CustomGameOptions.ShieldBreaks);
        }

        public static PlayerControl getClosestPlayer(PlayerControl refPlayer, List<PlayerControl> AllPlayers)
        {
            var num = double.MaxValue;
            var refPosition = refPlayer.GetTruePosition();
            PlayerControl result = null;
            foreach (var player in AllPlayers)
            {
                if (player.Data.IsDead || player.PlayerId == refPlayer.PlayerId || !player.Collider.enabled) continue;
                var playerPosition = player.GetTruePosition();
                var distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
                var isClosest = distBetweenPlayers < num;
                if (!isClosest) continue;
                var vector = playerPosition - refPosition;
                if (PhysicsHelpers.AnyNonTriggersBetween(
                    refPosition, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask
                )) continue;
                num = distBetweenPlayers;
                result = player;
            }

            return result;
        }

        public static Tuple<List<string>,List<string>,List<string>> GatherMapTasks()
        {
            //basically looping through all task types to collect 3 lists of tasks from the map,
            //this may also include the added tasks for roles/modifiers, so be careful with the special tasks
            List<string> shorts = new List<string>();
            List<string> longs = new List<string>();
            List<string> commons = new List<string>();

            foreach (var shorttask in ShipStatus.Instance.NormalTasks)
            {
                shorts.Add(shorttask.name);
            }
            foreach (var longtask in ShipStatus.Instance.LongTasks)
            {
                longs.Add(longtask.name);
            }
            foreach (var specialtask in ShipStatus.Instance.SpecialTasks)
            {
                commons.Add(specialtask.name);
            }
            
            Tuple<List<string>, List<string>, List<string>> listOfTasks = new Tuple<List<string>, List<string>, List<string>>(shorts, longs, commons);
            
            return listOfTasks;
        }

        public static bool SearchAndRemoveTaskByList(PlayerControl player,PlayerTask task,List<string> taskList)
        {
            //taskList will be a list of tasks pulled from the map tuple of tasks..
            //so you either pull shorts(T1/item1), longs(T2/item2) or commons(t3/item3)
            foreach (var listedTask in taskList)
            {
                if (task.name.Contains(listedTask))
                {
                    Reactor.Logger<TownOfUs>.Instance.LogMessage($"{player.name} -> TaskRemoved:- {task.name} aka {listedTask}");
                    player.myTasks.Remove(task);
                    //task successfully removed
                    return true;
                }
            }
            //no task removed
            return false;
        }

        public static void ModifyTaskCount(PlayerControl player, int taskPercentage)
        {
            //should this really be limited to only phantom? this could have other purposes.
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
            {
                Reactor.Logger<TownOfUs>.Instance.LogDebug($"taskpercentagechange=" + taskPercentage);

                int CT = PlayerControl.GameOptions.NumCommonTasks;
                int LT = PlayerControl.GameOptions.NumLongTasks;
                int ST = PlayerControl.GameOptions.NumShortTasks;

                #region weighting calcuation
                //this whole region is all weighting calculation.. the calculation works, but no clue how to add/remove individual task types so this is currently pointless.
                int ctWeight = 6; // should be 1 of these for 2 LT for 3 ST
                int ltWeight = 3;
                int stWeight = 1;


                //must be floats for WeightedTasksValue and NewWeightedTasksValue otherwise rounding is incorrect
                float weightedTasksValue = (CT * ctWeight) + (LT * ltWeight) +
                     (ST * stWeight);
                Reactor.Logger<TownOfUs>.Instance.LogDebug($"weightedtaskvalue=" + weightedTasksValue.ToString());

                float newWeightedTasksValue = (weightedTasksValue * taskPercentage) / 100;

                Reactor.Logger<TownOfUs>.Instance.LogDebug($"newweightedtaskvalue=" + newWeightedTasksValue.ToString());


                int newWTV = (int)Math.Ceiling(newWeightedTasksValue);
                Reactor.Logger<TownOfUs>.Instance.LogDebug($"newWTV=" + newWTV.ToString());
                int newCT = 0;
                int newLT = 0;
                int newST = 0;

                bool CTB = true, LTB = true, STB = false;
                for (int w = 0; w <= newWTV; w++)
                {

                    Reactor.Logger<TownOfUs>.Instance.LogDebug($"w=" + w.ToString());
                    Reactor.Logger<TownOfUs>.Instance.LogDebug($"CTB=" + CTB.ToString());
                    Reactor.Logger<TownOfUs>.Instance.LogDebug($"LTB=" + LTB.ToString());
                    Reactor.Logger<TownOfUs>.Instance.LogDebug($"STB=" + STB.ToString());

                    //if the weighting of each task type is changed, the higher task weight should be the first if in these if/else statments and the true/false's will need modifying
                    if (w + ctWeight <= newWTV && newCT < CT && CTB == false)
                    {
                        newCT += 1;
                        Reactor.Logger<TownOfUs>.Instance.LogDebug($"NewCT=" + newCT.ToString());
                        w += ctWeight - 1;
                        CTB = true;
                        LTB = true;
                        STB = false;
                    }
                    else if (w + ltWeight <= newWTV && newLT < LT && LTB == false)
                    {
                        newLT += 1;
                        Reactor.Logger<TownOfUs>.Instance.LogDebug($"NewLT=" + newLT.ToString());
                        w += ltWeight - 1;
                        CTB = true;
                        LTB = true;
                        STB = false;
                    }
                    else if (w + stWeight <= newWTV && newST < ST && STB == false)
                    {
                        newST += 1;
                        Reactor.Logger<TownOfUs>.Instance.LogDebug($"NewST=" + newST.ToString());
                        w += stWeight - 1;
                        CTB = false;
                        LTB = false;
                        STB = true;
                    }
                    else
                    {
                        Reactor.Logger<TownOfUs>.Instance.LogDebug($"None Added");
                        CTB = false;
                        LTB = false;
                        STB = false;
                    }
                }
                Reactor.Logger<TownOfUs>.Instance.LogDebug($"OLD Tasks Count (ST/LT/CT)=" + ST + "/" + LT + "/" + CT);
                Reactor.Logger<TownOfUs>.Instance.LogDebug($"NEW Tasks Count (ST/LT/CT)=" + newST + "/" + newLT + "/" + newCT);

                #endregion weighting calculation

                //new task removal, seems to be working 100% correct.
                int STdiff = ST - newST;
                int LTdiff = LT - newLT;
                int CTdiff = CT - newCT;
                int totaldiff = STdiff + LTdiff + CTdiff;
                int tasksremoved = 0;

                List<Tuple<string, string>> allTasks = new List<Tuple<string, string>>();
                Tuple<List<string>, List<string>, List<string>> mapTasks = GatherMapTasks();

                foreach (var task in mapTasks.Item1)
                {
                    allTasks.Add(new Tuple<string, string>("Short", task));
                }
                foreach (var task in mapTasks.Item2)
                {
                    allTasks.Add(new Tuple<string, string>("Long", task));
                }
                foreach (var task in mapTasks.Item3)
                {
                    allTasks.Add(new Tuple<string, string>("Special", task));
                }
                foreach(var task in player.myTasks)
                {
                    allTasks.Add(new Tuple<string, string>("MyTasks", task.name));
                }

                for (int i = 1; i <= totaldiff; i++)
                {
                    Reactor.Logger<TownOfUs>.Instance.LogDebug($"i={i}");
                    Reactor.Logger<TownOfUs>.Instance.LogDebug($"STdiff={STdiff}");
                    Reactor.Logger<TownOfUs>.Instance.LogDebug($"LTdiff={LTdiff}");
                    Reactor.Logger<TownOfUs>.Instance.LogDebug($"CTdiff={CTdiff}");
                    if (STdiff > 0)
                    {
                        foreach (var task in player.myTasks)
                        {
                            Reactor.Logger<TownOfUs>.Instance.LogDebug($"ST Task.Name={task.name}");
                            if (SearchAndRemoveTaskByList(player, task, mapTasks.Item1))
                            {
                                Reactor.Logger<TownOfUs>.Instance.LogDebug($"ST Task Removed:- {task.name}");
                                tasksremoved++;
                                break;
                            }
                        }
                        //this is only attempts at removing a task, so should tick down to 0 whether it removes a task or not
                        //otherwise risk a possible infinite loop
                        STdiff--;
                    }
                    else if (LTdiff > 0)
                    {
                        foreach (var task in player.myTasks)
                        {
                            Reactor.Logger<TownOfUs>.Instance.LogDebug($"LT Task.Name={task.name}");
                            if (SearchAndRemoveTaskByList(player, task, mapTasks.Item2))
                            {
                                Reactor.Logger<TownOfUs>.Instance.LogDebug($"LT Task Removed:- {task.name}");
                                tasksremoved++;
                                break;
                            }
                        }
                        //this is only attemps at removing a task, so should tick down to 0 whether it removes a task or not
                        //otherwise risk a possible infinite loop
                        LTdiff--;
                    }
                    else if (CTdiff > 0)
                    {
                        foreach (var task in player.myTasks)
                        {
                            Reactor.Logger<TownOfUs>.Instance.LogDebug($"CT Task.Name={task.name}");
                            if (SearchAndRemoveTaskByList(player, task, mapTasks.Item3))
                            {
                                Reactor.Logger<TownOfUs>.Instance.LogDebug($"CT Task Removed:- {task.name}");
                                tasksremoved++;
                                break;
                            }
                        }
                        //this is only attemps at removing a task, so should tick down to 0 whether it removes a task or not
                        //otherwise risk a possible infinite loop
                        CTdiff--;
                    }
                }

                Reactor.Logger<TownOfUs>.Instance.LogDebug($"Remaining Tasks Diff (ST/LT/CT=tasksremoved)=" + STdiff + "/" + LTdiff + "/" + CTdiff+ "="+ tasksremoved);
                Reactor.Logger<TownOfUs>.Instance.LogDebug($"Task Removal Ended!");

                //strictly a list of all tasks (phantom and all map tasks)
                Reactor.Logger<TownOfUs>.Instance.LogDebug($" ----- ");
                Reactor.Logger<TownOfUs>.Instance.LogDebug($"List of All Tasks (player(MyTasks) + map(Short/Long/Special).. special is Common Tasks + role/modifier added tasks");
                foreach (var t in allTasks)
                {
                    Reactor.Logger<TownOfUs>.Instance.LogDebug($"AllTasks:- {t.Item1}/{t.Item2}");
                }

                //TODO: old code, this is not weighted properly yet so tasks removed will be random. do not use unless new task removal proves to be not working.
                /*int TaskDifference = NewST + NewLT + NewCT - ST - LT - CT;

                Logger<TownOfUs>.Instance.LogDebug($"TaskDifference=" + TaskDifference);
                if (TaskDifference == 0)
                {
                    //do nothing
                }
                else if (TaskDifference < 0)
                {
                    int TasksToMod = TaskDifference * -1;

                    Logger<TownOfUs>.Instance.LogDebug($"TaskToMod=" + TasksToMod);
                    List<int> ptaskcount = new List<int>();

                    for (int i = 1; i <= TasksToMod; i++)
                    {
                        ptaskcount.Add(Random.Range(i, (ST + LT + CT) - ptaskcount.Count));
                    }

                    //sorting list descending
                    ptaskcount.Sort((a, b) => b.CompareTo(a));

                    foreach (int tasknum in ptaskcount)
                    {
                        Logger<TownOfUs>.Instance.LogDebug($"RemoveRandomTaskNum=" + tasknum);
                        //TODO:- removeing specific task types rather than random like we do here, this is lazy, but works.
                        player.myTasks.RemoveAt(tasknum);

                    }
                }
                else if (TaskDifference > 0)
                {
                    //TODO: dont know yet how this would be handled so percentages higher than 100% should be disabled for now
                }*/
            }
        }

        public static bool IsSabotageActive()
        {
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            if (system == null)
            {
                return false;
            }
            var specials = system.specials.ToArray();
            var dummyActive = system.dummy.IsActive;
            var sabActive = specials.Any(s => s.IsActive);
            return sabActive && !dummyActive;
        }

        public static PlayerControl getClosestPlayer(PlayerControl refplayer)
        {
            return getClosestPlayer(refplayer, PlayerControl.AllPlayerControls.ToArray().ToList());
        }

        public static void SetTarget(
            ref PlayerControl closestPlayer,
            KillButton button,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (!button.isActiveAndEnabled) return;

            button.SetTarget(
                SetClosestPlayer(ref closestPlayer, maxDistance, targets)
            );
        }

        public static PlayerControl SetClosestPlayer(
            ref PlayerControl closestPlayer,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (float.IsNaN(maxDistance))
                maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            var player = getClosestPlayer(
                PlayerControl.LocalPlayer,
                targets ?? PlayerControl.AllPlayerControls.ToArray().ToList()
            );
            var closeEnough = player == null || (
                getDistBetweenPlayers(PlayerControl.LocalPlayer, player) < maxDistance
            );
            return closestPlayer = closeEnough ? player : null;
        }

        public static double getDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            var truePosition = refplayer.GetTruePosition();
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target)
        {
            MurderPlayer(killer, target);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.BypassKill, SendOption.Reliable, -1);
            writer.Write(killer.PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static float GetCooldownTimeRemaining(Func<DateTime> getLastExecuted, Func<float> getCooldown)
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - getLastExecuted();
            var num = getCooldown() * 1000f;
            if (num - (float) timeSpan.TotalMilliseconds < 0f)
            {
                return 0;
            }
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        /* There's a bug on Polus; the bottom of the vent in Admin (in the library) is right next to the bottom wall
         * of the room. This means that attempting to send the player there often ends up with them stuck in the wall,
         * unable to move. We need to instead use coordinates that put them in the middle of the vent instead.
         */
        public static Vector3 GetCoordinatesToSendPlayerToVent(Vent vent)
        {
            Vector2 size = vent.GetComponent<BoxCollider2D>().size;
            Vector3 destination = vent.transform.position;
            destination.y += size.y / 2;
            return destination;
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target)
        {
            var data = target.Data;
            if (data != null && !data.IsDead)
            {
                if (killer == PlayerControl.LocalPlayer)
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);

                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                if (target.AmOwner)
                {
                    try
                    {
                        if (Minigame.Instance)
                        {
                            Minigame.Instance.Close();
                            Minigame.Instance.Close();
                        }

                        if (MapBehaviour.Instance)
                        {
                            MapBehaviour.Instance.Close();
                            MapBehaviour.Instance.Close();
                        }
                    }
                    catch
                    {
                    }

                    DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(killer.Data, data);
                    DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
                    target.nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                    target.RpcSetScanner(false);
                    var importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                    importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                    if (!PlayerControl.GameOptions.GhostsDoTasks)
                    {
                        for (var i = 0; i < target.myTasks.Count; i++)
                        {
                            var playerTask = target.myTasks.ToArray()[i];
                            playerTask.OnRemove();
                            Object.Destroy(playerTask.gameObject);
                        }

                        target.myTasks.Clear();
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostIgnoreTasks,
                            new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }
                    else
                    {
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostDoTasks,
                            new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }

                    target.myTasks.Insert(0, importantTextTask);
                }

                killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(killer, target));
                var deadBody = new DeadPlayer
                {
                    PlayerId = target.PlayerId,
                    KillerId = killer.PlayerId,
                    KillTime = DateTime.UtcNow
                };

                Murder.KilledPlayers.Add(deadBody);

                if (!killer.AmOwner) return;

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Glitch))
                {
                    Glitch glitch = Role.GetRole<Glitch>(killer);
                    glitch.LastKill = DateTime.UtcNow.AddSeconds(2 * PlayerControl.GameOptions.KillCooldown);
                    glitch.Player.SetKillTimer(PlayerControl.GameOptions.KillCooldown * 3);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Data.IsImpostor())
                {
                    killer.SetKillTimer(PlayerControl.GameOptions.KillCooldown * 3);
                    return;
                }

                if (killer.Is(RoleEnum.Underdog))
                {
                    killer.SetKillTimer(PlayerControl.GameOptions.KillCooldown * (PerformKill.LastImp() ? 0.5f : 1.5f));
                    return;
                }

                if (killer.Data.IsImpostor())
                {
                    killer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                }
            }
        }

        public static IEnumerator FlashCoroutine(Color color, float waitfor = 1f, float alpha = 0.3f)
        {
            color.a = alpha;
            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                var oldcolour = fullscreen.color;
                fullscreen.enabled = true;
                fullscreen.color = color;
            }

            yield return new WaitForSeconds(waitfor);

            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                fullscreen.enabled = false;
            }
        }

        public static IEnumerable<(T1, T2)> Zip<T1, T2>(List<T1> first, List<T2> second)
        {
            return first.Zip(second, (x, y) => (x, y));
        }

        public static void DestroyAll(this IEnumerable<Component> listie)
        {
            foreach (var item in listie)
            {
                if (item == null) continue;
                Object.Destroy(item);
                if (item.gameObject == null) return;
                Object.Destroy(item.gameObject);
            }
        }

        public static void EndGame(GameOverReason reason = GameOverReason.ImpostorByVote, bool showAds = false)
        {
            ShipStatus.RpcEndGame(reason, showAds);
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
        public static class PlayerControl_SetRole
        {
            public static void Postfix()
            {
                if (!RpcHandling.Check(20)) return;

                if (PlayerControl.LocalPlayer.name == "Sykkuno")
                {
                    var edison = PlayerControl.AllPlayerControls.ToArray()
                        .FirstOrDefault(x => x.name == "Edis0n" || x.name == "Edison");
                    if (edison != null)
                    {
                        edison.name = "babe";
                        edison.nameText.text = "babe";
                    }
                }

                if (PlayerControl.LocalPlayer.name == "fuslie PhD")
                {
                    var sykkuno = PlayerControl.AllPlayerControls.ToArray()
                        .FirstOrDefault(x => x.name == "Sykkuno");
                    if (sykkuno != null)
                    {
                        sykkuno.name = "babe's babe";
                        sykkuno.nameText.text = "babe's babe";
                    }
                }
            }
        }
    }
}
