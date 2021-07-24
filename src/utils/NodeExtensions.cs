using System;
using Godot;

namespace Parry2.utils
{
  public static class NodeExtensions
  {
    public static void DebugPrint(this Node node, string msg)
    {
      GD.Print($"[{node.Name}] {msg}");
    }

    public static void DebugPrintErr(this Node node, string msg)
    {
      GD.PrintErr($"[{node.Name}] {msg}");
    }
#nullable enable

    [Obsolete]
    public static T? GetNodeOrNull<T>(this Node node, NodePath? path) where T : Node =>
        path is null ? null : node.GetNodeOrNull<T>(path);

    public static AnimationNodeStateMachinePlayback GetPlaybackFrom(
      this Node parent, string path = "AnimationTree")
    {
      var player = parent.GetNode<AnimationTree>(path);

      if (player == null) throw new Exception("player is null");
      var playback = (AnimationNodeStateMachinePlayback) player.Get("parameters/playback");

      if (playback == null) throw new Exception("playback is null");

      return playback;
    }


    // Sibling node getters
    public static T GetSibling<T>(this Node node, NodePath path) where T : Node =>
        (T) node.GetSibling(path);

    public static T? GetSiblingOrNull<T>(this Node node, NodePath path) where T : Node =>
        node.GetSiblingOrNull(path) as T;

    public static Node GetSiblingOrNull(this Node node, NodePath path) =>
        node.GetParent().GetNodeOrNull(path);

    public static Node GetSibling(this Node node, NodePath path) =>
        node.GetParent().GetNode(path);
  }
}