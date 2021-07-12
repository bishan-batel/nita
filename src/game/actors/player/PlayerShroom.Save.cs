using System;
using System.Runtime.Serialization;
using Godot;
using Parry2.managers.save;
using Parry2.utils;

namespace Parry2.game.actors.player
{
    public partial class PlayerShroom : IPersistant, IGlobalPersistant
    {
        // Global Persistence 
        public string UniqueName => nameof(PlayerShroom);

        // TODO Global persistence functionality for when player can get new abilities
        public void GlobalLoad(ISerializable data)
        {
        }

        public ISerializable GlobalSave() => null;

        // Normal Persistence
        public ISerializable Save()
        {
            return new PlayerShroomData
            {
                Position = Position,
            };
        }

        public void LoadFrom(ISerializable obj)
        {
            if (!(obj is PlayerShroomData save)) return;
            Position = save.Position;
        }

        [Serializable]
        internal class PlayerShroomData : ISerializable
        {
            public Vector2Serial Position;

            public PlayerShroomData(SerializationInfo info = null, StreamingContext context = default)
            {
                if (info is null) return;
                Position = (Vector2Serial) info.GetValue(nameof(Position), typeof(Vector2Serial));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(Position), Position);
            }
        }

        [Serializable]
        public class PlayerShroomGlobalData : ISerializable
        {
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}