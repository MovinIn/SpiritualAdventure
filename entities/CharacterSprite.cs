using Godot;
using System;

namespace SpiritualAdventure.entities;

public partial class CharacterSprite : AnimatedSprite2D
{
	public void updateRotation(float direction)
	{
		Scale = getScale(direction);
	}

	public Vector2 getScale(float direction)
	{
		if (direction== 0) return new Vector2(Scale.X,Scale.Y);
		return new Vector2(Math.Abs(Scale.X)*Math.Sign(direction),Scale.Y);
	}
}
