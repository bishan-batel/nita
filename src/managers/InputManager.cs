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
		// AttackRotation = walkInput.Angle() + Mathf.Pi,
		AttackJustPressed = Input.IsActionJustPressed("attack"),
		WalkInput = walkInput,

		Owner = controller.Owner
	  };
	}

	static float GetAttackRot(Node2D controller)
	{
	  float GetMouseRotation()
	  {
		// if (controller is null) return 0f;
		float angle = controller.GetLocalMousePosition().Angle();
		GD.Print(angle);
		return angle;
	  }

	  float GetControllerRotation()
	  {
		float angle = new Vector2(
		  Input.GetJoyAxis(0, (int)JoystickList.Axis2),
		  Input.GetJoyAxis(0, (int)JoystickList.Axis3)
		).Angle();
		GD.Print(angle);
		return angle;
	  }

	  return OptionsManager.Options.ControllerMode == OptionsData.ControllerModeE.Mouse
		  // Mouse aiming
		  ? GetMouseRotation()

		  // Controller aiming
		  : GetControllerRotation();
	}
  }
}
