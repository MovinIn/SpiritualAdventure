using Godot;

namespace SpiritualAdventure.utility;

public static class GameUnitUtils
{
  public const int Scalar=16;
  
  public static Vector2 Vector2(float x, float y)
  {
    return new Vector2(x,y)*Scalar;
  }

  public static Vector2 FromPixels(Vector2 pixelVector)
  {
    return pixelVector * 1 / Scalar;
  }

  public static Vector2 ToPixels(Vector2 gameUnitVector)
  {
    return gameUnitVector * Scalar;
  }
}