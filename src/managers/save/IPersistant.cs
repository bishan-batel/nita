using System.Runtime.Serialization;

namespace Parry2.managers.save
{
  public interface IPersistant
  {
    public ISerializable Save();
    public void LoadFrom(ISerializable obj);
  }
}