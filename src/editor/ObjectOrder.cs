using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Parry2.game.actors.npc.bluehat;
using Parry2.game.actors.npc.bullshroom;
using Parry2.game.detail.foliage.glowing.chloropom;
using Parry2.game.world.actors.player;
using Parry2.game.world.objects.card;
using Parry2.game.world.objects.card.cardusers;
using Parry2.game.world.objects.checkpoint;
using Parry2.game.world.objects.ilkspring;
using Parry2.game.world.objects.saw;
using Parry2.game.world.objects.shroomvine_wheel;
using Parry2.game.world.objects.sporevine;
using Parry2.game.world.tilemaps;

namespace Parry2.editor
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