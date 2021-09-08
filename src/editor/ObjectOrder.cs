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
using Nita.game.world.terrain.Platform;
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
      typeof(Platform),
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
      int z = GetLayer(node.GetType());

      if (z != -1)
      {
        node.ZIndex = z;
        return;
      }

      // If the search failed, double check with script name (crude way I know but it's better than making every
      // node I want to be ordered have a tool script attached)
      if (node.GetScript() is not CSharpScript script) return;

      string name = script
          .ResourcePath
          .Replace(".cs", "")
          .Split("/")
          .Last()
          .ToLower()
          .Trim();
      z = Order.FindIndex(type => type.Name.ToLower() == name);
      if (z != -1) node.ZIndex = z;
    }

    public static int GetLayer(Type type) => Order.IndexOf(type);
  }
}