using System;
using Godot;

namespace Parry2.managers.sound
{
  public class SoundManager : Node
  {
    public static SoundManager Singleton;

    static AudioStreamPlayer _currentMusicPlayerPlayer;

    public static AudioStreamPlayer CurrentMusicPlayer
    {
      private set
      {
        if (value is null) return;

        if (CurrentMusicPlayer is null)
        {
          _currentMusicPlayerPlayer = value;
          Singleton.AddChild(CurrentMusicPlayer);
          return;
        }

        _currentMusicPlayerPlayer.QueueFree();
        _currentMusicPlayerPlayer = value;
        Singleton.AddChild(CurrentMusicPlayer);
      }

      get => _currentMusicPlayerPlayer;
    }

    public override void _Ready()
    {
      Singleton = this;
    }

    public static void Play(MusicSettings settings)
    {
      var player = new AudioStreamPlayer
      {
          Playing = true,
          Autoplay = true
      };
      settings.ApplySettings(player);
      CurrentMusicPlayer = player;
    }
  }

  public class MusicSettings : Resource
  {
    [Export] public readonly AudioStream Stream;
    [Export] [Obsolete] public bool Looping;
    [Export] public float PitchScale = 1f, VolumeDb;

    public MusicSettings(AudioStream stream) =>
        Stream = stream;

    public void ApplySettings(AudioStreamPlayer player)
    {
      player.PitchScale = PitchScale;
      player.VolumeDb = VolumeDb;
      player.Stream = Stream;
    }
  }
}