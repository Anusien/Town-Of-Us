using System;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.CrewmateRoles.HaunterMod
{
    public enum HaunterCanBeClickedBy
    {
        All,
        NonCrew,
        ImpsOnly
    }
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAnSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => SetHaunter.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class SetHaunter
    {
        public static PlayerControl WillBeHaunter;
        public static Vector2 StartPosition;

        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.exiled?.Object;
            if (!PlayerControl.LocalPlayer.DataIsDead && exiled != PlayerControl.LocalPlayer) return;
            if (exiled == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Is(RoleEnum.Jester)) return;
            if (PlayerControl.LocalPlayer != WillBeHaunter) return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Haunter))
            {
                Role.RoleDictionary.Remove(PlayerControl.LocalPlayer.PlayerId);
                var role = new Haunter(PlayerControl.LocalPlayer);
                role.RegenTask();
                Lights.SetLights();

                RemoveTasks(PlayerControl.LocalPlayer);
                PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();

                System.Console.WriteLine("Become Haunter - Haunter");

                PlayerControl.LocalPlayer.gameObject.layer = layerMask.NameToLayer("Players");

                var writer = AmongUsClient.Instance.StartRpcImmediatly(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.HaunterDied, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            if (Role.GetRole<Haunter>(PlayerControl.LocalPlayer).Caught) return;
            var startingVent =
                ShipStatus.Instance.AllVents[Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];
            if (PlayerControl.GameOptions.MapId == 2)
            {
                while (startingVent == ShipStatus.Instance.AllVents[5])
                {
                    startingVent = ShipStatus.Instance.AllVents[Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];
                }
            }
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(startingVent.transform.position);
            PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        public static void RemoveTasks(PlayerControl player)
        {
            var totalTasks = PlayerControl.GameOptions.NumCommonTasks + PlayerControl.GameOptions.NumLongTasks + 
                             PlayerControl.GameOptions.NumShortTasks;


            foreach (var task in player.myTasks)
                if (task.TryCast<NormalPlayerTask>() != null)
                {
                    var normalPlayerTask = task.Cast<NormalPlayerTask>();

                    var updateArrow = normalPlayerTask.taskStep > 0;

                    normalPlayerTask.taskStep = 0;
                    normalPlayerTask.Initialize();
                    if (normalPlayerTask.TaskType == TaskTypes.PickUpTowels)
                        foreach (var console in Object.FindObjectsOfType<TownlTaskConsole>())
                            console.Image.color = Color.white;
                    normalPlayerTask.taskStep = 0;
                    if (normalPlayerTask.TaskType == TaskTypes.UploadData)
                        normalPlayerTask.taskStep = 1;
                    if (updateArrow)
                        normalPlayerTask.UpdateArrow();

                        var taskInfo = player.Data.FindTaskById(task.Id);
                        taskInfo.Complete = false;
                }                 
        }

        /*public static void ResetTowels(NormalPlayerTask task)
        {
            var towelTask = task.Cast<TowelTask>();
            var data = new byte[8];
            var array = Enumerable.Range(0, 14).ToList();
            array.Shuffle();
            var b3 = 0;
            while (b3 < data.Length)
            {
                data[b3] = (byte)array[b3];
                b3++;
            }

            towelTask.Data = data;
            return;
        }

        public static void ResetRecords(NormalPlayerTask task)
        {
            task.Data = new
        }*/

        public static void AddColider(Haunter role)
        {
            var player = role.Player;
            var collider2d = player.gameObject.AddComponent<BoxCollider2D>();
            collider2d.isTrigger = true;
            var button = player.gameObject.AddComponent<PassiveButton>();
            button.OnClick = new Button.ButtonClickedEvent();
            button.OnMouseOut = new Button.ClickedEvent();
            button.OnMouseOver = new Button.ButtonClickedEvent();

            button.OnClick.AddListener((Action) ()) =>
            {
                if (MeetingHud.Instance) return;
                if (PlayerControl.LocalPlayer.Data.IsDead) return;
                if (CustomGameOptions.HaunterCanBeClickedBy == HaunterCanBeClickedBy.ImpsOnly && !PlayerControl.LocalPlayer.Data.IsImpostor()) return;
                if (CustomGameOptions.HaunterCanBeClickedBy == HaunterCanBeClickedBy.NonCrew && !(PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(RoleEnum.Shifter) || PlayerControl.LocalPlayer.Is(RoleEnum.Glitch) || PlayerControl.LocalPlayer.Is(RoleEnum.Executioner) || PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))) return;
                var taskinfos = player.Data.Tasks.ToArray();
                var tasksLeft = taskinfos.Count(x => !x.Complete);
                if (tasksLeft <= CustomGameOptions.HaunterTasksRemainingClicked)
                {
                    role.Caught = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.CatchHaunter, SendOption.Reliable, -1);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }));
        }
    }
}