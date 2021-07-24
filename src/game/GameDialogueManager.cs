using System;
using System.Reactive.Linq;
using Godot;
using GodotRx;

namespace Parry2.game
{
  public class GameDialogueManager : Control
  {
    public override void _Ready()
    {
      this
          .OnInput()
          .Where(@event => @event is InputEventKey)
          .Cast<InputEventKey>()
          .Where(key => key.IsActionPressed("debug_dialogue"))
          .Subscribe(async _ =>
          {
            Node node = DialogicSharp.Start("test_timeline");
            AddChild(node);

            await this.WaitNextIdleFrame();
            var control = node.GetNode<Control>("DialogNode");

            await this.WaitNextIdleFrame();
            control.RectSize = new Vector2(1600, 900);
            control.RectScale = Vector2.One * .188f;
          })
          .DisposeWith(this);
    }
  }
}