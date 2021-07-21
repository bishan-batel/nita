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
                )
            };

        static float GetAttackRot(Node2D controller)
        {
            float attackRot = Input.GetConnectedJoypads().Count > 0
                // Controller aiming
                ? new Vector2(
                    Input.GetJoyAxis(0, (int) JoystickList.Axis2),
                    Input.GetJoyAxis(0, (int) JoystickList.Axis3)
                ).Angle()
                // Mouse aiming
                : controller
                    ?.GetAngleTo(controller.GetGlobalMousePosition()) ?? 0f;
            return attackRot;
        }
    }
}