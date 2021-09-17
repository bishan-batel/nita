using System;
using System.Runtime.Serialization;
using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;
using Nita.managers.save;
using Nita.utils;

namespace Nita.game.world.objects.checkpoint
{
  [Group(SaveManager.PersistGroup)]
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

    Checkpoint ClaimedNode => ClaimedPath is null
        ? null
        : GameplayScene.CurrentRoom.GetNodeOrNull<Checkpoint>(_claimedPath);

    public override void _Ready() => this.Wire();

    /// <summary>
    /// Retrieves spawn location from the current claimed checkpoint
    /// </summary>
    public Vector2 GetSpawnLocation() => ClaimedNode.GlobalPosition;

    /// <summary>
    /// Called to clear checkpoint / to reset checkpoint once you leave a room
    /// </summary>
    public void OnLeftRoom() => ClaimedPath = null;

    public void Claim(Checkpoint checkpoint) => ClaimedPath = checkpoint.GetPathFromRoom();

    public ISerializable Save()
    {
      if (ClaimedPath is null) return null;

      this.DebugPrint("Saving checkpoint data");
      var save = new CheckpointManagerSave
      {
        Path = ClaimedPath
      };
      return save;
    }

    public void LoadFrom(ISerializable obj)
    {
      if (obj is not CheckpointManagerSave save) return;
      ClaimedPath = save.Path;
    }

    [Serializable]
    public struct CheckpointManagerSave : ISerializable
    {
      public string Path;

      public CheckpointManagerSave(SerializationInfo info, StreamingContext context) =>
          Path = info.GetString(nameof(Path));

      public void GetObjectData(SerializationInfo info, StreamingContext context) =>
          info.AddValue(nameof(Path), Path);
    }
  }
}