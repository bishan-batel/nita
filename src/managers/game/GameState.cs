using Godot;

namespace Parry2.managers.game
{
    public abstract class GameState : Node
    {
        protected GameStateManager Manager;

        protected GameState(string name) : this(null, name)
        {
        }

        protected GameState(GameStateManager manager, string name = "ROFL BOB")
        {
            Manager = manager;
            Name = name;
        }

        public override void _Ready()
        {
            Manager ??= GameStateManager.Singleton;
            Manager.CurrentState = this;
            // if (Manager.CurrentState != this)
            // throw new Exception(
            //     $"Multiple GameStates instances at once: " +
            //     $"{this.Name} & {Manager.CurrentState.Name}"
            // );
        }
    }
}