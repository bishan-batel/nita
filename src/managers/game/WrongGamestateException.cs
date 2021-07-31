using System;

namespace Nita.managers.game
{
  public class WrongGamestateException : Exception
  {
    public override string Message => $"Invalid gamestate [{GameStateManager.CurrentState.Name}], expected [{Desired}]";
    public readonly string Desired;
    public WrongGamestateException(Type desired) => Desired = desired.Name;
  }
}