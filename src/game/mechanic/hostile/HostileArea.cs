using Godot;

namespace Parry2.game.mechanic.hostile
{
    public class HostileArea : Area2D, IHostile
    {
        [Export] public bool Disabled { get; set; }
        [Export] public bool TeleportToSafeSpot { get; set; }
        [Export] public int Damage { get; set; }
    }
}