namespace TownOfUs.Roles
{
    public class Bomber : RoleWithCooldown
    {
        private KillButton _plantBombButton;

        public Bomber(PlayerControl player) : base(player, RoleEnum.Bomber, CustomGameOptions.BomberCooldown)
        {
            ImpostorText = () => "Wire crewmates to explode";
            TaskText = () => "Wire crewmates to explode";
        }

        public KillButton PlantBombButton
        {
            get => _plantBombButton;
            set
            {
                _plantBombButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}