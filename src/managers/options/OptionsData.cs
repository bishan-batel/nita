using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nita.managers.options
{
  [Serializable]
  public class OptionsData : ISerializable
  {
    public readonly Dictionary<string, object> Data = new();
    public readonly OptionsValue<ControllerModeE> ControllerMode;
    public readonly OptionsValue<bool> FullScreen;

    // Default options
    public OptionsData()
    {
      ControllerMode = (nameof(ControllerMode), ControllerModeE.Controller, Data);
      FullScreen = (nameof(FullScreen), true, Data);
    }

    // Serialization

    public OptionsData(SerializationInfo info, StreamingContext context)
    {
      Data = (Dictionary<string, object>)info.GetValue(nameof(Data), typeof(Dictionary<string, object>));

      ControllerMode = (nameof(ControllerMode), Data);
      FullScreen = (nameof(FullScreen), Data);
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue(nameof(Data), Data);
    }

    public enum ControllerModeE
    {
      Mouse,
      Controller
    }
  }
}