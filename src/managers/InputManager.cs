using Godot;
using Parry2.game.world.actors.player;

namespace Parry2.managers
{
    public static class InputManager
    {
        public static InputController Apply(InputController controller) =>
            new()
            {
                JumpPressed = Input.IsActionPressed("jump"),
                JumpJustPressed = Input.IsActionJustPressed("jump"),

                AttackRotation = GetAttackRot(controller.Owner),
                AttackJustPressed = Input.IsActionJustPressed("attack"),

                WalkInput = new Vector2(
                    Input.GetActionStrength("right") - Input.GetActionStrength("left"),
                    Input.GetActionStrength("down") - Input.GetActionStrength("up")
                ),
                Owner = controller.Owner
            };

        static float GetAttackRot(Node2D controller)
        {
            return Input.GetConnectedJoypads().Count is 0
                // Mouse aiming
                ? controller?.GetAngleTo(controller.GetGlobalMousePosition()) ?? 0f

                // Controller aiming
                : new Vector2(
                    Input.GetJoyAxis(0, (int) JoystickList.Axis2),
                    Input.GetJoyAxis(0, (int) JoystickList.Axis3)
                ).Angle();
        }
    }
}