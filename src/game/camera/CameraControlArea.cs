using Godot;

namespace Parry2.game.camera
{
    public class CameraControlArea : Area2D
    {
        [Export] public Vector2 CameraZoom;
        [Export] public NodePath Point;
    }
}