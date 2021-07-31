using System.Runtime.Serialization;

namespace Nita.managers.save
{
  public interface IPersistant
  {
    public ISerializable Save();
    public void LoadFrom(ISerializable obj);
  }
}