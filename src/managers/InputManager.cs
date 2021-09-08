using System;
using Godot;
using Nita.game.world.actors.player;
using Nita.managers.options;

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
        AttackJustPressed = Input.IsActionJustPressed("attack"),
        WalkInput = walkInput,

        // passes owner down
        Owner = controller.Owner
      };
    }

    static float GetAttackRot(Node2D controller)
    {
      float angle;
      if (Options.GetOrDefault(Option.UseController, false))
      {
        // Controller aiming
        angle = new Vector2(
          Input.GetJoyAxis(0, (int)JoystickList.Axis2),
          Input.GetJoyAxis(0, (int)JoystickList.Axis3)
        ).Angle();
        return angle;
      }

      // Mouse Aiming

      // getting player position in screenspace coords
      Transform2D viewportTransform = controller.GetViewport().CanvasTransform;

      // translates viewport to be based around viewport2D position instead of (0,0)
      Vector2 viewScreenCoord = controller.GlobalPosition + viewportTransform.origin;

      // Scales it from pixel viewport size to window viewport size

      // TODO find out why I can't use WinUpscaleFactor and have to calculate the scale per frame
      // viewScreenCoord *= Global.WinUpscaleFactor;
      // viewScreenCoord.x *= Global.WinUpscaleFactor;
      // viewScreenCoord.y *= Global.WinUpscaleFactor;

      Vector2 winSize = Global.UpScaledWindowSize;
      viewScreenCoord.x *= winSize.x / Global.WindowSize.width;
      viewScreenCoord.y *= winSize.y / Global.WindowSize.height;

      // screenspace mouse position
      Vector2 screenMouseCoord = controller.GetViewport().GetMousePosition();

      angle = screenMouseCoord.AngleToPoint(viewScreenCoord);
      return angle;
    }
  }
}