using System;
using System.Linq;
using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;
using Godot.Collections;
using Parry2.editor;
using Parry2.game.world.tilemaps;

namespace Parry2.game.world.objects.card
{
  // TODO fix this class / add electricity arc effect
  [Tool]
  public class KeyCard : Node2D
  {
    [Node("AnimationPlayer")] readonly AnimationPlayer _player = null;
    CardColor _color;

    Array<Node> _electricity;
    Timer _timer;

    public static Shader ElectricityShader =>
        ResourceLoader.Load<Shader>("res://assets/materials/shaders/electric.shader");

    [Export]
    public CardColor Color
    {
      set
      {
        _color = value;
        if (!IsInsideTree()) return;
        GetNode<Sprite>("Sprite").Frame = (int) value;

        Connections
            .ToList()
            .ForEach(path => GetNode<ICardUser>(path).Color = value);
      }
      get => _color;
    }

    [Export] public Array<NodePath> Connections { set; get; } = new();

    public override void _Ready()
    {
      this.Wire();
      _player.Play("idle");

      Color = Color;

#if DEBUG
      if (Engine.EditorHint) return;
#endif
      _electricity = new Array<Node>();

      // Connections
      //     .ToList()
      //     .ForEach(path =>
      //     {
      //         Node node = _createElectricity(GlobalPosition, GetNode<Node2D>(path).GlobalPosition);
      //
      //         GetParent().CallDeferred("add_child", node);
      //         _electricity.Add(node);
      //     });
    }

    public void Entered(object param)
    {
      _player.Play("collected");
      AddChild(_timer = new Timer());
      _timer.Start(.1f);
      _timer.Connect("timeout", this, nameof(Timeout));
    }

    public void Timeout()
    {
      Connections
          .ToList()
          .ForEach(path => GetNode<ICardUser>(path).OnCardCollected());

      _electricity
          .ToList()
          .ForEach(node => node.QueueFree());
      QueueFree();
    }

    [Obsolete]
    Node _createElectricity(Vector2 from, Vector2 to)
    {
      var rect = new Sprite
      {
          Material = new ShaderMaterial {Shader = ElectricityShader}
      };

      Vector2 diff = to - from;
      Vector2 midpoint = (to + from) * .5f;

      rect.GlobalPosition = midpoint;
      rect.GlobalRotation = diff.Angle();
      rect.Texture = GetNode<Sprite>(nameof(Sprite)).Texture;
      rect.Scale = new Vector2(
        diff.Length() / rect.Texture.GetWidth(),
        100f / rect.Texture.GetHeight());
      rect.ZIndex = ObjectOrder.GetLayer(typeof(BackgroundMap));
      rect.ZAsRelative = false;

      return rect;
    }
  }

  public enum CardColor
  {
    White,
    Red,
    Purple,
    Green,
    Yellow
  }
}