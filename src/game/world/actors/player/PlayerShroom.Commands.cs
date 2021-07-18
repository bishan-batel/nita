using Godot;
using Parry2.debug;

namespace Parry2.game.world.actors.player
{
    public partial class PlayerShroom
    {
        bool _noclip;

        public bool NoClip
        {
            set => GetNode<CollisionShape2D>("CollisionBody").Disabled = _noclip = value;
            get => _noclip;
        }

        void _addDebugCommands()
        {
            this.AddCommand(
                "noclip",
                "Puts player in noclip mode",
                nameof(CmdNoClip)
            );
        }

        public void CmdNoClip()
        {
        }
    }
}