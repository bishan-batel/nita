using Godot;
using Parry2.game.mechanic.hostile;

namespace Parry2.game.world.tilemaps
{
    public class SpikesMap : TileMap, IHostile
    {
        [Export] public bool Disabled { get; set; }
        [Export] public bool TeleportToSafeSpot { get; set; }
        [Export] public int Damage { get; set; }
    }
}