using System.Runtime.Serialization;

namespace Parry2.managers.save
{
  public interface IGlobalPersistant
  {
    string UniqueName { get; }
    void GlobalLoad(ISerializable data);
    ISerializable GlobalSave();
  }
}