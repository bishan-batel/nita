using Godot;
using Godot.Collections;

namespace Nita.managers.sound.sfx
{
  // TODO add randomization for sound
  [Tool]
  public class Sfx2D : AudioStreamPlayer2D
  {
    [Export] public Array<AudioStream> Streams { set; get; }

    // TODO make cur ves work in SFX random 
    // [Export] public Curve RandomCurve { set; get; }

    [Export] public GraphEdit Edit;

    // ReSharper disable once UnusedMember.Local
    void random_play() => RandomPlay();

    public void RandomPlay()
    {
      GD.Randomize();

      // Chooses random stream based on curve
      // float idxPercent = (RandomCurve.Interpolate(GD.Randf()) - RandomCurve.MinValue) / RandomCurve.MaxValue;
      float idxPercent = GD.Randf();
      var idx = (int)(idxPercent * Streams.Count);
      Stream = Streams[idx];
      
      // Chooses random pitch based on curve

      Play();
    }
  }
}