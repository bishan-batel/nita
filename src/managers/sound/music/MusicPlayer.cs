using System;
using GDMechanic.Wiring;
using Godot;
using Nita.addons.godotrx;
using Nita.utils;
using Object = Godot.Object;

namespace Nita.managers.sound.music
{
  public class MusicPlayer : AudioStreamPlayer
  {
    public const float MinDb = -60;

    [Export(PropertyHint.None, "Seconds until player starts")]
    public float AutoplayOffsetStart { set; get; }

    [Export] public new bool Autoplay { set; get; }

    // TODO fix fade in curves
    // [Export] public Curve FadeInCurve { set; get; }
    [Export] public float FadeInDuration, SoundStartPosition;

    [Export(PropertyHint.Range, "-60, 24")]
    public new float VolumeDb { set; get; }

    public override void _Ready()
    {
      this.Wire();
      // FadeInCurve = new Curve();
      base.Autoplay = false;

      // Autoplay functionality 
      if (!Autoplay) return;


      if (AutoplayOffsetStart != 0f)
      {
        this.Dispatch(FadeIn, AutoplayOffsetStart);
        return;
      }

      FadeIn();
    }

    // For connecting with signals
    // ReSharper disable once InconsistentNaming
    public void trigger(params object[] rofl) => FadeIn();

    // ReSharper disable once InconsistentNaming
    public void trigger() => FadeIn();

    public void FadeIn()
    {
      base.VolumeDb = MinDb;
      float time = 0;

      EmitSignal(nameof(OnPlay));
      Play();

      IDisposable disposable = this.OnProcess().Subscribe(delta =>
      {
        time += delta;
        float percentComplete = time / FadeInDuration;

        // float interpolatedCurve = _curveValAt(percentComplete);
        // GD.Print($"{percentComplete}% ic: {interpolatedCurve} t {FadeInCurve.Interpolate(1f)}");

        float curvedDb = Mathf.Lerp(MinDb, VolumeDb, percentComplete);
        base.VolumeDb = Mathf.Min(curvedDb, FadeInDuration);
      });

      this.Dispatch(() =>
      {
        disposable.Dispose();
        base.VolumeDb = VolumeDb;
      }, FadeInDuration);
    }

    [Signal]
    public delegate void OnPlay();
  }
}