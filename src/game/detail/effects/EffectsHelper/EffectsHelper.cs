using System;
using System.Linq;
using Godot;

namespace Nita.game.detail.effects.EffectsHelper
{
  public class EffectsHelper : Node
  {
    public const string GetPlayerPosGroup = "Effect_GetPlayerPos";

    public override void _Ready()
    {
      GetTree()
          .GetNodesInGroup(GetPlayerPosGroup)
          .Cast<Node2D>()
          .ToList()
          .ForEach(node =>
          {
            try
            {
              var shaderMat = (ShaderMaterial) node.Material;
              shaderMat.SetShaderParam("globalTransform", node.GlobalPosition);
            }
            catch (InvalidCastException)
            {
              throw new ShaderNotFoundException();
            }
          });
    }

    public override void _Process(float delta)
    {
      // UpdatePlayerPos();
    }

    public void UpdatePlayerPos()
    {
      Vector2 playerPosition = GameplayScene.CurrentRoom.Player.GlobalPosition;
      GetTree()
          .GetNodesInGroup(GetPlayerPosGroup)
          .Cast<Node2D>()
          .ToList()
          .ForEach(node =>
          {
            try
            {
              var shaderMat = (ShaderMaterial) node.Material;
              shaderMat.SetShaderParam("playerPos", playerPosition);
            }
            catch (InvalidCastException)
            {
              throw new ShaderNotFoundException();
            }
          });
    }

    public class ShaderNotFoundException : Exception
    {
      public override string Message => $"{nameof(ShaderMaterial)} not found in node";
    }
  }
}