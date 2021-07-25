using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
          .Subscribe(async _ => await Start("test_timeline"))
          .DisposeWith(this);
    }

    public async Task<Node> Start(string timeline)
    {
      Node node = DialogicSharp.Start(timeline);
      AddChild(node);

      await this.WaitNextIdleFrame();
      var control = node.GetNode<Control>("DialogNode");

      await this.WaitNextIdleFrame();
      control.RectSize = new Vector2(Global.UpscaledWindowSize.width, Global.UpscaledWindowSize.height);
      control.RectScale = Vector2.One * Global.WinUpscaleFactor;

      return node;
    }
  }
}