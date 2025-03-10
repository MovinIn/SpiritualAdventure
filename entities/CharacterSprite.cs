using Godot;
using System;

namespace SpiritualAdventure.entities;

public partial class CharacterSprite : AnimatedSprite2D
{
  public static class Direction
  {
    public const int Left = -1;
    public const int Right = 1;
  }
  
  public void UpdateRotation(float direction)
  {
    Scale = GetScale(direction);
  }

  public Vector2 GetScale(float direction)
  {
    if (direction== 0) return new Vector2(Scale.X,Scale.Y);
    return new Vector2(Math.Abs(Scale.X)*Math.Sign(direction),Scale.Y);
  }
}