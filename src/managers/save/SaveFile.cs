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
        internal const SaveVersion LatestSaveVersion = SaveVersion.V0W0B;
        internal SaveVersion Version;

        public SaveFile(string name = "debug_save")
        {
            Name = name;
            CurrentRoomName = RoomList.GetName("test_room");
            GlobalData = new Dictionary<string, ISerializable>();
            RoomData = new Dictionary<string, RoomSaveData>
            {
                {
                    "rofl_test", new RoomSaveData()
                }
            };

            Version = LatestSaveVersion;
        }

        public SaveFile(SerializationInfo info, StreamingContext context)
        {
            string versionString = info.GetString(nameof(Version));
            bool success = Enum.TryParse(versionString, out Version);
            if (!success) throw new Exception($"Invalid version {versionString}");

            CurrentRoomName = info.GetString(nameof(CurrentRoomName));
            Name = info.GetString(nameof(Name));

            Type roomDataType = typeof(Dictionary<string, RoomSaveData>);
            RoomData = info.GetValue(nameof(RoomData), roomDataType) as Dictionary<string, RoomSaveData>;

            Type globalDataType = typeof(Dictionary<string, ISerializable>);
            GlobalData =
                info.GetValue(nameof(GlobalData), globalDataType) as Dictionary<string, ISerializable>;
        }

        public string CurrentRoomName { get; }
        public Dictionary<string, RoomSaveData> RoomData { get; }
        public Dictionary<string, ISerializable> GlobalData { get; }


        public PackedScene CurrentRoom =>
            RoomList.GetChapterScene(CurrentRoomName);

        public string Name { get; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Game data
            info.AddValue(nameof(CurrentRoomName), CurrentRoomName);
            info.AddValue(nameof(RoomData), RoomData);
            info.AddValue(nameof(GlobalData), GlobalData);

            // Meta
            info.AddValue(nameof(Version), Version.ToString());
            info.AddValue(nameof(Name), Name);
        }

        // Serialize to file
        public bool Flush()
        {
            string path = SaveManager.FormatAbsPath(Name);

            var dir = new Directory();
            if (!dir.DirExists(SaveManager.SavesDirAbsPath))
                dir.MakeDir(SaveManager.SavesDirAbsPath);
            FileStream fs = File.Create(path);
            var bf = new BinaryFormatter();
            bf.Serialize(fs, this);
            fs.Close();
            return true;
        }

        // Deserialize specified file
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
            V0W0B
        }
    }
}