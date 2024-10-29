using Godot;
using SpiritualAdventure.entities;

public partial class Player : CharacterBody2D
{
	private const float Speed = 300.0f;
	private CharacterSprite sprite;

	public override void _Ready()
	{
		sprite=GetNode<CharacterSprite>("Sprite");
	}

	public override void _PhysicsProcess(double delta)
	{
		gameTick(delta);
	}

	private void gameTick(double delta){
		Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
		Velocity = inputDirection * Speed;

		idleOrElse(inputDirection==new Vector2(),"walk");
        sprite.updateRotation(Velocity.X);

		MoveAndSlide();
	}

	private void idleOrElse(bool idle,string animation){
		sprite.Play(idle?"idle":animation);
	}
}
