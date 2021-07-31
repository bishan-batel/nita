using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Nita.game.detail.foliage.glowing.chloropom;
using Nita.game.world.actors.npc.bluehat;
using Nita.game.world.actors.npc.bullshroom;
using Nita.game.world.actors.player;
using Nita.game.world.objects.card;
using Nita.game.world.objects.card.cardusers;
using Nita.game.world.objects.checkpoint;
using Nita.game.world.objects.ilkspring;
using Nita.game.world.objects.saw;
using Nita.game.world.objects.shroomvine_wheel;
using Nita.game.world.objects.sporevine;
using Nita.game.world.tilemaps;

namespace Nita.editor
{
  [Tool]
  public static class ObjectOrder
  {
    public const string LayeredGroup = "Layered";

    public static readonly List<Type> Order = new[]
    {
      typeof(BackgroundMap),
      typeof(KeyCard),
      typeof(ShroomvineWheel),
      typeof(BluehatNpc),
      typeof(PlayerShroom),
      typeof(Bullshroom),
      typeof(Ilkspring),
      typeof(Sporevine),
      typeof(Saw),
      typeof(Chloropom),
      typeof(Checkpoint),
      typeof(GroundMap),
      typeof(MechanicalDoor),
      typeof(MechanicalRotate)
    }.ToList();

    public static void OrganizeLayersInTree(SceneTree tree)
    {
      tree
          ?.GetNodesInGroup(LayeredGroup)
          .OfType<Node2D>()
          .ToList()
          .ForEach(Organize);
    }

    public static void Organize(this Node2D node)
    {
      if (node is null) return;
      node.ZAsRelative = false;
      node.ZIndex = GetLayer(node.GetType());

      // If the search failed, double check with script
      if (node.ZIndex != -1) return;

      if (node.GetScript() is not CSharpScript script) return;

      string name = script
          .ResourcePath
          .Replace(".cs", "")
          .Split("/")
          .Last()
          .ToLower()
          .Trim();
      node.ZIndex = Order.FindIndex(type => type.Name.ToLower() == name);
    }

    public static int GetLayer(object obj) =>
        GetLayer(obj.GetType());

    public static int GetLayer(Type type) =>
        Order.IndexOf(type);
  }
}