using System.Collections.Generic;
using System.Configuration;

namespace Nita.managers.options
{
  public class OptionsValue<T>
  {
    public readonly string Name;
    readonly Dictionary<string, object> _data;
    T _value;

    public OptionsValue(string name, Dictionary<string, object> data)
    {
      Name = name;
      _data = data;
      SetTo((T)_data[Name]);
    }

    public OptionsValue(string name, T value, Dictionary<string, object> data)
    {
      Name = name;
      _data = data;
      SetTo(value);
    }

    public void SetTo(T value)
    {
      _data.Remove(Name);
      _data.Add(Name, _value = value);
    }

    // Implicit operators, used only to make creating new options cleaner and code more readable
    // Converts tuples into constructors
    
    public static implicit operator OptionsValue<T>((string name, T value, Dictionary<string, object> dict) from) =>
        new(from.name, from.value, from.dict);

    public static implicit operator OptionsValue<T>((string name, Dictionary<string, object> dict) from) =>
        new(from.name, from.dict);

    public static implicit operator T(OptionsValue<T> from) => from._value;
  }
}