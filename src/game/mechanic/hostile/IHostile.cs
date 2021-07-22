using Godot;

namespace Parry2.game.mechanic.hostile
{
    public interface IHostile
    {
        [Export] public bool Disabled { set; get; }
        [Export] public bool TeleportToSafeSpot { set; get; }
        [Export] public int Damage { set; get; }
    }
}