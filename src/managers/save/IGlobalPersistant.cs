using System.Runtime.Serialization;

namespace Nita.managers.save
{
  public interface IGlobalPersistant
  {
    string UniqueName { get; }
    void GlobalLoad(ISerializable data);
    ISerializable GlobalSave();
  }
}