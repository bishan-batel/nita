using System;
using System.Runtime.Serialization;
using Godot;
using Parry2.managers.save;
using Parry2.utils;

namespace Parry2.game.world.objects.checkpoint
{
  public class CheckpointManagerNode : Node, IPersistant
  {
    NodePath _claimedPath;

    public NodePath ClaimedPath
    {
      set
      {
        if (value == _claimedPath) return;
        if (value is null)
        {
          _claimedPath = null;
          return;
        }

        GameplayScene.EnteredGate = null;

        // Unclaim old checkpoint and claim new one (visually)
        ClaimedNode?.UnClaim();

        _claimedPath = value;
        ClaimedNode?.Claim();

        SaveManager.SaveDeferred();
      }
      get => _claimedPath;
    }

    Checkpoint ClaimedNode =>
        _claimedPath is null ? null : GameplayScene.CurrentRoom.GetNodeOrNull<Checkpoint>(_claimedPath);

    public ISerializable Save()
    {
      if (ClaimedPath is null) return null;

      this.DebugPrint("Saving checkpoint data");
      var save = new CheckpointManagerSave {Path = ClaimedPath};
      return save;
    }

    public void LoadFrom(ISerializable obj)
    {
      if (obj is not CheckpointManagerSave save) return;
      ClaimedPath = save.Path;
    }

    public Vector2 GetSpawnLocation() =>
        ClaimedNode?.IsInsideTree() ?? false
            ? ClaimedNode.GlobalPosition
            : Vector2.Zero;

    public void OnLeftRoom()
    {
      ClaimedPath = null;
    }

    public void Claim(Checkpoint checkpoint)
    {
      Claim(checkpoint.GetPath());
    }

    public void Claim(NodePath checkpoint)
    {
      ClaimedPath = checkpoint;
    }

    public override void _Ready()
    {
      AddToGroup(SaveManager.PersistGroup);
    }

    [Serializable]
    public struct CheckpointManagerSave : ISerializable
    {
      public string Path;

      public CheckpointManagerSave(SerializationInfo info, StreamingContext context) =>
          Path = info.GetString(nameof(Path));

      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
        info.AddValue(nameof(Path), Path);
      }
    }
  }
}