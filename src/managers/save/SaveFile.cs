using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Godot;
using Parry2.game.room;
using Directory = Godot.Directory;
using File = System.IO.File;
using RoomSaveData = System.Collections.Generic.Dictionary<string, System.Runtime.Serialization.ISerializable>;

namespace Parry2.managers.save
{
  [Serializable]
  public class SaveFile : ISerializable
  {
    internal const SaveVersion LatestSaveVersion = SaveVersion.V0A;
    internal SaveVersion Version;

    public string CurrentRoomName { get; }
    public Dictionary<string, RoomSaveData> RoomData { get; }
    public Dictionary<string, ISerializable> GlobalData { get; }

    public PackedScene CurrentRoom =>
        RoomList.GetChapterScene(CurrentRoomName);

    public string Name { get; }

    /// <summary>
    /// Creates a new save file, will not save to file unless explicitly told so
    /// </summary>
    /// <param name="name">Filename for </param>
    public SaveFile(string name = "debug_save")
    {
      if (!SaveManager.IsValidFileName(name)) throw new IOException($"Invalid name {name}");

      Name = name;
      CurrentRoomName = RoomList.GetName("test_room");
      GlobalData = new Dictionary<string, ISerializable>();
      RoomData = new Dictionary<string, RoomSaveData>();

      Version = LatestSaveVersion;
    }

    /// <summary>
    /// Used for serialization, do not use explicitly
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    /// <exception cref="Exception"></exception>
    public SaveFile(SerializationInfo info, StreamingContext context)
    {
      // Gets version 
      string versionString = info.GetString(nameof(Version));
      bool success = Enum.TryParse(versionString, out Version);
      if (!success) throw new IOException($"Invalid save version {versionString}");

      CurrentRoomName = info.GetString(nameof(CurrentRoomName));
      Name = info.GetString(nameof(Name));

      Type roomDataType = typeof(Dictionary<string, RoomSaveData>);
      RoomData = info.GetValue(nameof(RoomData), roomDataType) as Dictionary<string, RoomSaveData>;

      Type globalDataType = typeof(Dictionary<string, ISerializable>);
      GlobalData =
          info.GetValue(nameof(GlobalData), globalDataType) as Dictionary<string, ISerializable>;
    }


    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      // Game data
      info.AddValue(nameof(CurrentRoomName), CurrentRoomName);
      info.AddValue(nameof(RoomData), RoomData);
      info.AddValue(nameof(GlobalData), GlobalData);

      // Meta
      info.AddValue(nameof(Version), (int) Version);
      info.AddValue(nameof(Name), Name);
    }

    /// <summary>
    /// Serializes content and saves it to filesystem
    /// </summary>
    /// <returns>Success</returns>
    public bool Flush()
    {
      // try
      // {
      string path = SaveManager.FormatAbsPath(Name);

      SaveManager.OpenSaveDirectory();
      FileStream fs = File.Create(path);
      var bf = new BinaryFormatter();
      bf.Serialize(fs, this);
      fs.Close();
      return true;
    }

    /// <summary>
    /// Opens & deserializes specified file 
    /// </summary>
    /// <param name="path">Full path to file</param>
    public static SaveFile Open(string path)
    {
      FileStream fs = File.OpenRead(path);
      var bf = new BinaryFormatter();
      object data = bf.Deserialize(fs);
      fs.Close();
      return data as SaveFile;
    }

    [Serializable]
    internal enum SaveVersion
    {
      Invalid,
      V0A
    }
  }
}