using Godot;
using Parry2.managers;

namespace Parry2.game.world.actors.player
{
    public struct InputController
    {
        public bool JumpPressed { set; get; }

        public bool JumpJustPressed { set; get; }

        public bool AttackJustPressed { set; get; }

        public Vector2 WalkInput { set; get; }

        public float AttackRotation { set; get; }

        public Node2D Owner { set; get; }

        public InputController(Node2D playerShroom)
        {
            Owner = playerShroom;

            JumpPressed = JumpJustPressed = AttackJustPressed = false;
            WalkInput = Vector2.Zero;
            AttackRotation = 0;
        }

        public void Set(InputController controller)
        {
            JumpPressed = controller.JumpPressed;
            JumpJustPressed = controller.JumpJustPressed;
            AttackJustPressed = controller.AttackJustPressed;
            WalkInput = controller.WalkInput;
            AttackRotation = controller.AttackRotation;
        }

        public void Clear()
        {
            JumpPressed = JumpJustPressed = AttackJustPressed = false;
            WalkInput = Vector2.Zero;
            AttackRotation = 0;
        }
    }
}