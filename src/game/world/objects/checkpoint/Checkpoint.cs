using System;
using System.Runtime.Serialization;
using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;
using Nita.managers.save;

namespace Nita.game.world.objects.checkpoint
{
  [Group(CheckpointGroup)]
  public class Checkpoint : Node2D, IPersistant
  {
	public const string CheckpointGroup = "Checkpoint";

	[Node("AnimationPlayer")] readonly AnimationPlayer _player = null;


	public ISerializable Save() =>
		null;

	public void LoadFrom(ISerializable obj)
	{
	  if (!(obj is CheckpointSave save)) return;

	  _player.Play(save.IsClaimed ? "unclaim" : "claim");
	}

	public override void _Ready()
	{
	  this.Wire();
	}

	public void Claim() => _player.Play("claim");

	public void UnClaim() => _player.Play("unclaim");

	// Notified checkpoint manager of checkpoint being hit
	public void Entered(Node body) =>
		GameplayScene
			.CurrentRoom
			.CheckpointManager
			.Claim(this);

	[Serializable]
	internal struct CheckpointSave : ISerializable
	{
	  public bool IsClaimed;

	  public CheckpointSave(bool claimed) =>
		  IsClaimed = claimed;

	  public CheckpointSave(SerializationInfo info, StreamingContext context) =>
		  IsClaimed = info.GetBoolean(nameof(IsClaimed));

	  public void GetObjectData(SerializationInfo info, StreamingContext context)
	  {
		info.AddValue(nameof(IsClaimed), IsClaimed);
	  }
	}
  }
}
