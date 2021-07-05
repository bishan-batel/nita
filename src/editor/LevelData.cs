using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Godot;
using TiledCS;

// TODO credit https://github.com/TheBoneJarmer/TiledCS
// TODO uh actually fucking work on the level editor you lazy piece of shit
// lol
namespace Parry2.editor
{
    [Obsolete("Incomplete class", true)]
    public class LevelData : ISerializable
    {
        public const string
            FileExtension = ".lmao",
            TilemapFileExtension = ".tmx";

        // Meta filesystem
        public readonly string Name, ParentDirectory;
        public string FileName => Name + FileExtension;
        public string Path => ParentDirectory.PlusFile(FileName);
        public string TiledPath => ParentDirectory.PlusFile(Name) + TilemapFileExtension;

        public TiledMap Map => new(TiledPath);

        /// <param name="path">Should be an absolute path with LevelData file extension</param>
        public LevelData(string path)
        {
            string[] directories = path.Split("/");
            string name = directories.Last();

            string fileName = path.Remove(0, path.Length - name.Length);
            ParentDirectory = path.Remove(path.Length - name.Length);
            Name = fileName.Remove(fileName.Length - FileExtension.Length);
        }

        public LevelData(string directory, string name)
        {
            ParentDirectory = directory;
            Name = name;
        }

        public void ApplyTo(Node root)
        {
            TiledMap map = Map;

            map.Layers.ToList().ForEach(l => { });
            TiledLayer layer = map.Layers[0];
        }

        // save filesystem garbo

        /// <summary>
        /// Saves the file to system
        /// </summary>
        void Flush()
        {
            FileStream fs = System.IO.File.Open(Path, FileMode.Create);
            var bf = new BinaryFormatter();
            bf.Serialize(fs, this);
            fs.Close();
        }

        public LevelData(SerializationInfo info, StreamingContext context = default)
        {
            ParentDirectory = info.GetString(nameof(ParentDirectory));
            Name = info.GetString(nameof(Name));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ParentDirectory), ParentDirectory);
            info.AddValue(nameof(Name), Name);
        }

        public override string ToString() =>
            $"[FullPath: '{Path}', " +
            $"Name: '{Name}', " +
            $"Parent Directory '{ParentDirectory}'" +
            $"TiledPath: '{TiledPath}'" +
            "]";
    }
}