using System;
using System.Diagnostics.CodeAnalysis;
using TownOfUs.Patches;
using UnityEngine.UIElements;

namespace TownOfUs.CustomOption
{
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Generate
    {
        private static CustomHeaderOption CrewmateRoles;
        public static CustomNumberOption MayorOn;
        public static CustomNumberOption LoversOn;
        public static CustomNumberOption SheriffOn;
        public static CustomNumberOption EngineerOn;
        public static CustomNumberOption SwapperOn;
        public static CustomNumberOption InvestigatorOn;
        public static CustomNumberOption TimeLordOn;
        public static CustomNumberOption MedicOn;
        public static CustomNumberOption SeerOn;
        public static CustomNumberOption SpyOn;
        public static CustomNumberOption SnitchOn;
        public static CustomNumberOption AltruistOn;
        public static CustomNumberOption ProphetOn;
        public static CustomNumberOption CovertOn;
        public static CustomNumberOption LighterOn;

        private static CustomHeaderOption NeutralRoles;
        public static CustomNumberOption JesterOn;
        public static CustomNumberOption ShifterOn;
        public static CustomNumberOption GlitchOn;
        public static CustomNumberOption ExecutionerOn;
        public static CustomNumberOption ArsonistOn;
        public static CustomNumberOption PhantomOn;

        private static CustomHeaderOption ImpostorRoles;
        public static CustomNumberOption JanitorOn;
        public static CustomNumberOption MorphlingOn;
        public static CustomNumberOption CamouflagerOn;
        public static CustomNumberOption MinerOn;
        public static CustomNumberOption SwooperOn;
        public static CustomNumberOption UndertakerOn;
        public static CustomNumberOption UnderdogOn;
        public static CustomNumberOption TeleporterOn;
        public static CustomNumberOption ConcealerOn;
        public static CustomNumberOption GrenadierOn;

        private static CustomHeaderOption Modifiers;
        public static CustomNumberOption TorchOn;
        public static CustomNumberOption DiseasedOn;
        public static CustomNumberOption FlashOn;
        public static CustomNumberOption TiebreakerOn;
        public static CustomNumberOption DrunkOn;
        public static CustomNumberOption BigBoiOn;
        public static CustomNumberOption ButtonBarryOn;
        public static CustomNumberOption AnthropomancerOn;
        public static CustomNumberOption CarnivoreOn;


        private static CustomHeaderOption CustomGameSettings;
        public static CustomNumberOption InitialImpostorKillCooldown;
        public static CustomToggleOption ColourblindComms;
        public static CustomToggleOption MeetingColourblind;
        public static CustomToggleOption ImpostorSeeRoles;
        public static CustomToggleOption DeadSeeRoles;
        public static CustomNumberOption MaxImpostorRoles;
        public static CustomNumberOption MaxNeutralRoles;
        public static CustomToggleOption RoleUnderName;
        public static CustomNumberOption VanillaGame;

        private static CustomHeaderOption Assassination;
        public static CustomNumberOption AssassinKills;
        public static CustomToggleOption AssassinGuessNeutrals;
        public static CustomToggleOption AssassinCrewmateGuess;
        public static CustomToggleOption AssassinMultiKill;


        private static CustomHeaderOption Mayor;
        public static CustomNumberOption MayorVoteBank;
        public static CustomToggleOption MayorAnonymous;

        private static CustomHeaderOption Lovers;
        public static CustomToggleOption BothLoversDie;
        public static CustomNumberOption LovingImpostorOn;

        private static CustomHeaderOption Sheriff;
        public static CustomToggleOption ShowSheriff;
        public static CustomToggleOption SheriffKillOther;
        public static CustomToggleOption SheriffKillsJester;
        public static CustomToggleOption SheriffKillsShifter;
        public static CustomToggleOption SheriffKillsGlitch;
        public static CustomToggleOption SheriffKillsExecutioner;
        public static CustomToggleOption SheriffKillsArsonist;
        public static CustomNumberOption SheriffKillCd;
        public static CustomToggleOption SheriffBodyReport;

        private static CustomHeaderOption Shifter;
        public static CustomNumberOption ShifterCd;
        public static CustomStringOption WhoShifts;

        private static CustomHeaderOption Engineer;
        public static CustomStringOption EngineerPer;

        private static CustomHeaderOption Investigator;
        public static CustomNumberOption FootprintSize;
        public static CustomNumberOption FootprintInterval;
        public static CustomNumberOption FootprintDuration;
        public static CustomToggleOption AnonymousFootPrint;
        public static CustomToggleOption VentFootprintVisible;

        private static CustomHeaderOption TimeLord;
        public static CustomToggleOption RewindRevive;
        public static CustomNumberOption RewindDuration;
        public static CustomNumberOption RewindCooldown;
        public static CustomToggleOption TimeLordVitals;

        private static CustomHeaderOption Medic;
        public static CustomStringOption ShowShielded;
        public static CustomToggleOption MedicReportSwitch;
        public static CustomNumberOption MedicReportNameDuration;
        public static CustomNumberOption MedicReportColorDuration;
        public static CustomStringOption WhoGetsNotification;
        public static CustomToggleOption ShieldBreaks;

        private static CustomHeaderOption Seer;
        public static CustomNumberOption SeerCooldown;
        public static CustomStringOption SeerInfo;
        public static CustomStringOption SeeReveal;
        public static CustomToggleOption NeutralRed;
        public static CustomNumberOption SeerCrewmateChance;
        public static CustomNumberOption SeerNeutralChance;
        public static CustomNumberOption SeerImpostorChance;

        private static CustomHeaderOption Snitch;
        public static CustomToggleOption SnitchOnLaunch;
        public static CustomToggleOption SnitchSeesNeutrals;
        public static CustomToggleOption SnitchSeesInMeetings;

        private static CustomHeaderOption Altruist;
        public static CustomNumberOption ReviveDuration;
        public static CustomToggleOption AltruistTargetBody;

        private static CustomHeaderOption Prophet;
        public static CustomNumberOption ProphetCooldown;
        public static CustomToggleOption ProphetInitialReveal;

        private static CustomHeaderOption Covert;
        public static CustomNumberOption CovertCooldown;
        public static CustomNumberOption CovertDuration;

        private static CustomHeaderOption Lighter;
        public static CustomNumberOption LighterCooldown;
        public static CustomNumberOption LighterDuration;

        private static CustomHeaderOption TheGlitch;
        public static CustomNumberOption MimicCooldownOption;
        public static CustomNumberOption MimicDurationOption;
        public static CustomNumberOption HackCooldownOption;
        public static CustomNumberOption HackDurationOption;
        public static CustomStringOption GlitchHackDistanceOption;

        private static CustomHeaderOption Arsonist;
        public static CustomNumberOption DouseCooldown;
        public static CustomToggleOption ArsonistGameEnd;

        private static CustomHeaderOption Phantom;
        public static CustomNumberOption PhantomTaskWin;

        private static CustomHeaderOption Executioner;
        public static CustomStringOption OnTargetDead;


        private static CustomHeaderOption Camouflager;
        public static CustomNumberOption CamouflagerCooldown;
        public static CustomNumberOption CamouflagerDuration;

        private static CustomHeaderOption Morphling;
        public static CustomNumberOption MorphlingCooldown;
        public static CustomNumberOption MorphlingDuration;

        private static CustomHeaderOption Miner;
        public static CustomNumberOption MineCooldown;

        private static CustomHeaderOption Swooper;
        public static CustomNumberOption SwoopCooldown;
        public static CustomNumberOption SwoopDuration;

        private static CustomHeaderOption Undertaker;
        public static CustomNumberOption DragCooldown;

        private static CustomHeaderOption Teleporter;
        public static CustomNumberOption TeleporterCooldown;
        public static CustomToggleOption TeleportSelf;
        public static CustomToggleOption TeleportOccupiedVents;

        private static CustomHeaderOption Concealer;
        public static CustomNumberOption ConcealCooldown;
        public static CustomNumberOption TimeToConceal;
        public static CustomNumberOption ConcealDuration;

        private static CustomHeaderOption Grenadier;
        public static CustomNumberOption GrenadeCooldown;
        public static CustomNumberOption GrenadeDuration;

        private static Func<object, string> PercentFormat { get; } = value => $"{value:0}%";
        private static Func<object, string> CooldownFormat { get; } = value => $"{value:0.0#}s";


        public static void GenerateAll()
        {
            var num = 0;

            Patches.ExportButton = new Export(num++);
            Patches.ImportButton = new Import(num++);


            #region Probabilities
            CrewmateRoles = new CustomHeaderOption(num++, "Crewmate Roles");
            MayorOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Mayor).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            LoversOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Lover).WrapTextInColor("Lovers")}", 0f, 0f, 100f, 10f,
                PercentFormat);
            SheriffOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Sheriff).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            EngineerOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Engineer).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            SwapperOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Swapper).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            InvestigatorOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Investigator).GetColoredName()}", 0f, 0f, 100f,
                10f, PercentFormat);
            TimeLordOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.TimeLord).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            MedicOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Medic).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            SeerOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Seer).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            SpyOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Spy).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            SnitchOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Snitch).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            AltruistOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Altruist).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            ProphetOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Prophet).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            CovertOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Covert).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            LighterOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Lighter).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);


            NeutralRoles = new CustomHeaderOption(num++, "Neutral Roles");
            JesterOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Jester).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            ShifterOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Shifter).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            GlitchOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Glitch).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            ExecutionerOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Executioner).GetColoredName()}", 0f, 0f, 100f,
                10f, PercentFormat);
            ArsonistOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Arsonist).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            PhantomOn = new CustomNumberOption(true, num++,
                $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Phantom).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);


            ImpostorRoles = new CustomHeaderOption(num++, "Impostor Roles");
            JanitorOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Janitor).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            MorphlingOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Morphling).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            CamouflagerOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Camouflager).GetColoredName()}", 0f, 0f, 100f,
                10f, PercentFormat);
            MinerOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Miner).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            SwooperOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Swooper).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            UndertakerOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Undertaker).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            UnderdogOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Underdog).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            TeleporterOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Teleporter).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            ConcealerOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Concealer).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);
            GrenadierOn = new CustomNumberOption(true, num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Grenadier).GetColoredName()}", 0f, 0f, 100f, 10f,
                PercentFormat);


            Modifiers = new CustomHeaderOption(num++, "Modifiers");
            TorchOn = new CustomNumberOption(true, num++, "<color=#FFFF99FF>Torch</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DiseasedOn =
                new CustomNumberOption(true, num++, "<color=#808080FF>Diseased</color>", 0f, 0f, 100f, 10f,
                    PercentFormat);
            FlashOn = new CustomNumberOption(true, num++, "<color=#FF8080FF>Flash</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TiebreakerOn = new CustomNumberOption(true, num++, "<color=#99E699FF>Tiebreaker</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DrunkOn = new CustomNumberOption(true, num++, "<color=#758000FF>Drunk</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            BigBoiOn = new CustomNumberOption(true, num++, "<color=#FF8080FF>Giant</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ButtonBarryOn =
                new CustomNumberOption(true, num++, "<color=#E600FFFF>Button Barry</color>", 0f, 0f, 100f, 10f,
                    PercentFormat);
            AnthropomancerOn =
                new CustomNumberOption(true, num++, "<color=#336629>Coroner</color>", 0f, 0f, 100f, 10f,
                    PercentFormat);
            CarnivoreOn =
                new CustomNumberOption(true, num++, "<color=#640000>Carnivore</color>", 0f, 0f, 100f, 10f, PercentFormat);
            #endregion


            #region GameSettings
            CustomGameSettings =
                new CustomHeaderOption(num++, "Custom Game Settings");
            InitialImpostorKillCooldown = new CustomNumberOption(num++, "Initial Impostor Kill Cooldown", 10f, 10f, 60f,
                2.5f, CooldownFormat);
            ColourblindComms = new CustomToggleOption(num++, "Camouflaged Comms", false);
            MeetingColourblind = new CustomToggleOption(num++, "Camouflaged Meetings", false);
            ImpostorSeeRoles = new CustomToggleOption(num++, "Impostors can see the roles of their team", false);
            DeadSeeRoles =
                new CustomToggleOption(num++, "Dead can see everyone's roles", false);
            MaxImpostorRoles =
                new CustomNumberOption(num++, "Max Impostor Roles", 1f, 1f, 3f, 1f);
            MaxNeutralRoles =
                new CustomNumberOption(num++, "Max Neutral Roles", 1f, 1f, 5f, 1f);
            RoleUnderName = new CustomToggleOption(num++, "Role Appears Under Name");
            VanillaGame = new CustomNumberOption(num++, "Probability of a completely vanilla game", 0f, 0f, 100f, 5f,
                PercentFormat);

            Assassination = new CustomHeaderOption(num++, "<color=#FF0000FF>Assassination</color>");
            AssassinKills = new CustomNumberOption(num++, "Number of Assassinations", 0, 1, 10, 1);
            AssassinCrewmateGuess = new CustomToggleOption(num++, "Impostors can guess \"Crewmate\"", false);
            AssassinGuessNeutrals = new CustomToggleOption(num++, "Impostors can guess Neutral roles", false);
            AssassinMultiKill = new CustomToggleOption(num++, "Impostors can assassinate more than once per meeting");
            #endregion

            #region CrewRoles
            Mayor =
                new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Mayor).GetColoredName()}");
            MayorVoteBank =
                new CustomNumberOption(num++, "Initial Mayor Vote Bank", 1, 1, 5, 1);
            MayorAnonymous =
                new CustomToggleOption(num++, "Mayor Votes Show Anonymous", false);

            Lovers =
                new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Lover).WrapTextInColor("Lovers")}");
            BothLoversDie = new CustomToggleOption(num++, "Both Lovers Die");
            LovingImpostorOn = new CustomNumberOption(num++, "Allow Loving Impostor",25f, 0f, 100f, 10f,
                PercentFormat);

            Sheriff =
                new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Sheriff).GetColoredName()}");
            ShowSheriff = new CustomToggleOption(num++, "Show Sheriff", false);
            SheriffKillOther =
                new CustomToggleOption(num++, "Sheriff Miskill Kills Crewmate", false);
            SheriffKillsJester =
                new CustomToggleOption(num++, "Sheriff Kills Jester", false);
            SheriffKillsShifter =
                new CustomToggleOption(num++, "Sheriff Kills Shifter", false);
            SheriffKillsGlitch =
                new CustomToggleOption(num++, "Sheriff Kills The Glitch", true);
            SheriffKillsExecutioner =
                new CustomToggleOption(num++, "Sheriff Kills Executioner", false);
            SheriffKillsArsonist =
                new CustomToggleOption(num++, "Sheriff Kills Arsonist", false);
            SheriffKillCd =
                new CustomNumberOption(num++, "Sheriff Kill Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            SheriffBodyReport = new CustomToggleOption(num++, "Sheriff can report who they've killed");

            Engineer =
                new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Engineer).GetColoredName()}");
            EngineerPer =
                new CustomStringOption(num++, "Engineer Fix Per", new[] {"Round", "Game"});

            Investigator =
                new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Investigator).GetColoredName()}");
            FootprintSize = new CustomNumberOption(num++, "Footprint Size", 4f, 1f, 10f, 1f);
            FootprintInterval =
                new CustomNumberOption(num++, "Footprint Interval", 1f, 0.25f, 5f, 0.25f, CooldownFormat);
            FootprintDuration = new CustomNumberOption(num++, "Footprint Duration", 10f, 1f, 10f, 0.5f, CooldownFormat);
            AnonymousFootPrint = new CustomToggleOption(num++, "Anonymous Footprint", false);
            VentFootprintVisible = new CustomToggleOption(num++, "Footprint Vent Visible", false);

            TimeLord =
                new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.TimeLord).GetColoredName()}");
            RewindRevive = new CustomToggleOption(num++, "Revive During Rewind", false);
            RewindDuration = new CustomNumberOption(num++, "Rewind Duration", 3f, 3f, 15f, 0.5f, CooldownFormat);
            RewindCooldown = new CustomNumberOption(num++, "Rewind Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            TimeLordVitals =
                new CustomToggleOption(num++, "Time Lord can use Vitals", false);

            Medic =
                new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Medic).GetColoredName()}");
            ShowShielded =
                new CustomStringOption(num++, "Show Shielded Player",
                    new[] {"Self", "Medic", "Self+Medic", "Everyone"});
            MedicReportSwitch = new CustomToggleOption(num++, "Show Medic Reports");
            MedicReportNameDuration =
                new CustomNumberOption(num++, "Time Where Medic Reports Will Have Name", 0, 0, 60, 2.5f,
                    CooldownFormat);
            MedicReportColorDuration =
                new CustomNumberOption(num++, "Time Where Medic Reports Will Have Color Type", 15, 0, 120, 2.5f,
                    CooldownFormat);
            WhoGetsNotification =
                new CustomStringOption(num++, "Who gets murder attempt indicator",
                    new[] {"Medic", "Shielded", "Everyone", "Nobody"});
            ShieldBreaks = new CustomToggleOption(num++, "Shield breaks on murder attempt", false);

            Seer =
                new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Seer).GetColoredName()}");
            SeerCooldown =
                new CustomNumberOption(num++, "Seer Cooldown", 25f, 10f, 100f, 2.5f, CooldownFormat);
            SeerInfo =
                new CustomStringOption(num++, "Info that Seer sees", new[] {"Role", "Team"});
            SeeReveal =
                new CustomStringOption(num++, "Who Sees That They Are Revealed",
                    new[] {"Crew", "Imps+Neut", "All", "Nobody"});
            NeutralRed =
                new CustomToggleOption(num++, "Neutrals show up as Impostors", false);
            SeerCrewmateChance = new CustomNumberOption(num++,
                "Chance to successfully reveal a Crewmate", 100f, 0f, 100f, 10f, PercentFormat);
            SeerNeutralChance = new CustomNumberOption(num++,
                "Chance to successfully reveal a Neutral role", 100f, 0f, 100f, 10f, PercentFormat);
            SeerImpostorChance = new CustomNumberOption(num++,
                "Chance to successfully reveal an Impostor", 100f, 0f, 100f, 10f, PercentFormat);

            Snitch = new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Snitch).GetColoredName()}");
            SnitchOnLaunch =
                new CustomToggleOption(num++, "Snitch knows who they are on Game Start", false);
            SnitchSeesNeutrals = new CustomToggleOption(num++, "Snitch sees neutral roles", false);
            SnitchSeesInMeetings = new CustomToggleOption(num++, "Snitch sees in meetings");

            Altruist = new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Altruist).GetColoredName()}");
            ReviveDuration =
                new CustomNumberOption(num++, "Altruist Revive Duration", 10, 1, 30, 1f, CooldownFormat);
            AltruistTargetBody =
                new CustomToggleOption(num++, "Target's body disappears on beginning of revive", false);

            Prophet = new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Prophet).GetColoredName()}");
            ProphetCooldown = new CustomNumberOption(num++, "Prophet Cooldown", 40f, 10f, 120f, 2.5f, CooldownFormat);
            ProphetInitialReveal =
                new CustomToggleOption(num++, "Prophet starts the game with a player revealed", false);

            Covert = new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Covert).GetColoredName()}");
            CovertCooldown = new CustomNumberOption(num++, "Covert Cooldown", 30f, 10f, 120f, 2.5f, CooldownFormat);
            CovertDuration = new CustomNumberOption(num++, "Covert Duration", 15f, 5f, 30f, 2.5f, CooldownFormat);

            Lighter = new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Lighter).GetColoredName()}");
            LighterCooldown = new CustomNumberOption(num++, "Lighter Cooldown", 20f, 5f, 60f, 2.5f, CooldownFormat);
            LighterDuration = new CustomNumberOption(num++, "Lighter Duration", 5f, 1f, 10f, 2.5f, CooldownFormat);
            #endregion


            #region NeutralRoles
            Shifter =
                new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Shifter).GetColoredName()}");
            ShifterCd =
                new CustomNumberOption(num++, "Shifter Cooldown", 30f, 10f, 60f, 2.5f, CooldownFormat);
            WhoShifts = new CustomStringOption(num++,
                "Who gets the Shifter role on Shift", new[] {"NoImps", "RegCrew", "Nobody"});

            TheGlitch =
                new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Glitch).GetColoredName()}");
            MimicCooldownOption = new CustomNumberOption(num++, "Mimic Cooldown", 30, 10, 120, 2.5f, CooldownFormat);
            MimicDurationOption = new CustomNumberOption(num++, "Mimic Duration", 10, 1, 30, 1f, CooldownFormat);
            HackCooldownOption = new CustomNumberOption(num++, "Hack Cooldown", 30, 10, 120, 2.5f, CooldownFormat);
            HackDurationOption = new CustomNumberOption(num++, "Hack Duration", 10, 1, 30, 1f, CooldownFormat);
            GlitchHackDistanceOption =
                new CustomStringOption(num++, "Glitch Hack Distance", new[] {"Short", "Normal", "Long"});

            Executioner =
                new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Executioner).GetColoredName()}");
            OnTargetDead = new CustomStringOption(num++, "Executioner becomes on Target Dead",
                new[] {"Crew", "Jester"});

            Arsonist = new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Arsonist).GetColoredName()}");
            DouseCooldown =
                new CustomNumberOption(num++, "Douse Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            ArsonistGameEnd = new CustomToggleOption(num++, "Game keeps going so long as Arsonist is alive", false);
            Phantom = new CustomHeaderOption(num++, "<color=#662962>Phantom</color>");
            PhantomTaskWin =
                new CustomNumberOption(num++, "Tasks To Win", 75, 50, 100, 5f, PercentFormat);
            #endregion


            #region ImpostorRoles
            Morphling =
                new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Morphling).GetColoredName()}");
            MorphlingCooldown =
                new CustomNumberOption(num++, "Morphling Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            MorphlingDuration =
                new CustomNumberOption(num++, "Morphling Duration", 10, 5, 15, 1f, CooldownFormat);

            Camouflager =
                new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Camouflager).GetColoredName()}");
            CamouflagerCooldown =
                new CustomNumberOption(num++, "Camouflager Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            CamouflagerDuration =
                new CustomNumberOption(num++, "Camouflager Duration", 10, 5, 15, 1f, CooldownFormat);

            Miner = new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Miner).GetColoredName()}");
            MineCooldown =
                new CustomNumberOption(num++, "Mine Cooldown", 25, 10, 40, 2.5f, CooldownFormat);

            Swooper = new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Swooper).GetColoredName()}");
            SwoopCooldown =
                new CustomNumberOption(num++, "Swoop Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            SwoopDuration =
                new CustomNumberOption(num++, "Swoop Duration", 10, 5, 15, 1f, CooldownFormat);

            Undertaker = new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Undertaker).GetColoredName()}");
            DragCooldown = new CustomNumberOption(num++, "Drag Cooldown", 25, 10, 40, 2.5f, CooldownFormat);

            Teleporter = new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Teleporter).GetColoredName()}");
            TeleporterCooldown =
                new CustomNumberOption(num++, "Teleporter Cooldown", 45, 10, 120, 2.5f, CooldownFormat);
            TeleportSelf = new CustomToggleOption(num++, "Teleport Teleports Themself", true);
            TeleportOccupiedVents = new CustomToggleOption(num++, "Allow Occupied Vents", true);

            Concealer = new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Concealer).GetColoredName()}");
            ConcealCooldown = new CustomNumberOption(num++, "Conceal Cooldown", 30, 10, 60, 2.5f, CooldownFormat);
            TimeToConceal = new CustomNumberOption(num++, "Delay Before Concealing", 5, 2.5f, 15, 2.5f, CooldownFormat);
            ConcealDuration = new CustomNumberOption(num++, "Conceal Duration", 10, 2.5f, 20f, 2.5f, CooldownFormat);

            Grenadier =
                new CustomHeaderOption(num++, $"{RoleDetailsAttribute.GetRoleDetails(RoleEnum.Grenadier).GetColoredName()}");
            GrenadeCooldown =
                new CustomNumberOption(num++, "Flash Grenade Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            GrenadeDuration =
                new CustomNumberOption(num++, "Flash Grenade Duration", 10, 5, 15, 1f, CooldownFormat);
            #endregion
        }
    }
}
