using Godot;
using Nita.debug;

namespace Nita.game.world.actors.player
{
  public partial class PlayerShroom
  {
    bool _noClip;

    public bool NoClip
    {
      set
      {
        _noClip = value;
        GetNode<CollisionShape2D>("CollisionBody").Disabled = value;
      }
      get => _noClip;
    }

    public float NoClipSpeed { get; set; } = 150f;

    void _addDebugCommands()
    {
      this.AddCommand(
        "noclip",
        "Puts player in noclip mode",
        nameof(CmdNoClip)
      );

      this.AddCommand(
        "noclipspeed",
        "Changes speed of noclip",
        nameof(CmdNoClipSpeed),
        ("speed", Variant.Type.Real)
      );
    }

    public void CmdNoClipSpeed(float speed)
    {
      NoClipSpeed = speed;
    }

    public void CmdNoClip()
    {
      NoClip = !NoClip;
      GConsole.WriteLine($"Toggled noclip mode to [color=green]{NoClip}");
    }
  }
}