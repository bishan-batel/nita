using Godot;

namespace Parry2.managers.game
{
    public abstract class GameState : Node
    {
        protected GameState(string name = "ROFL BOB")
        {
            Name = name;
        }

        public override void _Ready()
        {
            GameStateManager.CurrentState = this;
            // if (Manager.CurrentState != this)
            // throw new Exception(
            //     $"Multiple GameStates instances at once: " +
            //     $"{this.Name} & {Manager.CurrentState.Name}"
            // );
        }
    }
}