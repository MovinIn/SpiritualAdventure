using Godot;
using System;
using SpiritualAdventure.entities;

public partial class Npc : AnimatableBody2D
{
	protected const float SPEED = 120f;
	protected CharacterSprite sprite;

	public override void _Ready()
	{
		sprite=GetNode<CharacterSprite>("Sprite");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		sprite.updateRotation(ConstantLinearVelocity.X);
	}
}
