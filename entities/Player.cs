using Godot;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;

public partial class Player : CharacterBody2D
{
	private const float Speed = 300.0f;
	private CharacterSprite sprite;

    private static PackedScene scene=ResourceLoader.Load<PackedScene>("res://entities/Player.tscn");

    public static Player Instantiate()
    {
      return scene.Instantiate<Player>();
    }
    
	public override void _Ready()
	{
		sprite=GetNode<CharacterSprite>("Sprite");
	}

	public override void _PhysicsProcess(double delta)
	{
		gameTick(delta);
	}

	private void gameTick(double delta)
    {
      if (Level.Paused()) return;
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
