using Godot;
using Nita.editor;

namespace Nita.game
{
  [Tool]
  public class LayerHelper : Node
  {
#if DEBUG
    Timer _timer;
#endif

    [Export] public float OrganizeInterval = 1.0f;

    // Used as a sort of button within the godot editor
    [Export]
    public bool OrganizeLayers
    {
      // ReSharper disable once ValueParameterNotUsed
      set => ObjectOrder.OrganizeLayersInTree(IsInsideTree() ? GetTree() : null);
      get => false;
    }

    public override void _Ready()
    {
      OrganizeLayers = true;
#if DEBUG
      if (!Engine.EditorHint) return;
      AddChild(_timer = new Timer {OneShot = true});
      _timer.Connect("timeout", this, nameof(Timeout));
      _timer.Start(OrganizeInterval);
#endif
    }

#if DEBUG
    public void Timeout()
    {
      OrganizeLayers = true;
      _timer.Start(OrganizeInterval);
    }
#endif
  }
}