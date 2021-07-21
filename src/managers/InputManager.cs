using Godot;
using Parry2.game.world.actors.player;
using Parry2.utils;

namespace Parry2.managers
{
    public static class InputManager
    {
        public static InputController Apply(InputController controller)
        {
            float attackRot = Input.GetConnectedJoypads().Count > 0
                // Controller aiming
                ? new Vector2(
                    Input.GetJoyAxis(0, (int) JoystickList.Axis2),
                    Input.GetJoyAxis(0, (int) JoystickList.Axis3)
                ).Angle()
                // Mouse aiming
                : controller.Owner
                    ?.GetAngleTo(controller.Owner.GetGlobalMousePosition()) ?? 0f;

            controller.Set(new InputController
            {
                JumpPressed = Input.IsActionPressed("jump"),
                JumpJustPressed = Input.IsActionJustPressed("jump"),

                AttackRotation = attackRot,
                AttackJustPressed = Input.IsActionJustPressed("attack"),

                WalkInput = new Vector2(
                    Input.GetActionStrength("right") - Input.GetActionStrength("left"),
                    Input.GetActionStrength("down") - Input.GetActionStrength("up")
                )
            });

            return controller;
        }
    }
}