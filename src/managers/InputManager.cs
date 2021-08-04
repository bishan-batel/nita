using Godot;
using Nita.game.world.actors.player;

namespace Nita.managers
{
  public static class InputManager
  {
    public static InputController Apply(InputController controller)
    {
      var walkInput = new Vector2(
        Input.GetActionStrength("right") - Input.GetActionStrength("left"),
        Input.GetActionStrength("down") - Input.GetActionStrength("up")
      );

      return new InputController
      {
        JumpPressed = Input.IsActionPressed("jump"),
        JumpJustPressed = Input.IsActionJustPressed("jump"),

        AttackRotation = GetAttackRot(controller.Owner),
        // AttackRotation = walkInput.Angle() + Mathf.Pi,
        AttackJustPressed = Input.IsActionJustPressed("attack"),
        WalkInput = walkInput,

        Owner = controller.Owner
      };
    }

    static float GetAttackRot(Node2D controller) =>
        Input.GetConnectedJoypads().Count is 0
            // Mouse aiming
            ? controller?.GetAngleTo(controller.GetGlobalMousePosition()) ?? 0f

            // Controller aiming
            : new Vector2(
              Input.GetJoyAxis(0, (int) JoystickList.Axis2),
              Input.GetJoyAxis(0, (int) JoystickList.Axis3)
            ).Angle();
  }
}