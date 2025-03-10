using Godot;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.utility;

public partial class Player : CharacterBody2D
{
  private const float Speed = 300.0f;
  private CharacterSprite sprite;
  private Camera2D camera;
  
  private static PackedScene scene=ResourceLoader.Load<PackedScene>("res://entities/Player.tscn");

  public static Player Instantiate()
  {
    return scene.Instantiate<Player>();
  }
    
  public override void _Ready()
  {
    sprite=GetNode<CharacterSprite>("Sprite");
    camera=GetNode<Camera2D>("Camera2D");
  }

  public override void _PhysicsProcess(double delta)
  {
    GameTick(delta);
  }

  private void GameTick(double delta)
  {
    if (Level.Paused()) return;
    if (Level.currentCameraMode!=Level.CameraMode.Player && 
        Level.currentCameraMode!=Level.CameraMode.Ghost)
    {
      IdleOrElse(true);
      return;
    }
    camera.MakeCurrent();
    
    Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
    Velocity = inputDirection * Speed;

    IdleOrElse(inputDirection==new Vector2(),"walk");
    sprite.updateRotation(Velocity.X);

    switch (Level.currentCameraMode)
    {
      case Level.CameraMode.Player:
        MoveAndSlide();
        break;
      case Level.CameraMode.Ghost:
        Translate(Velocity*(float)delta);
        break;
    }
  }

  private void IdleOrElse(bool idle,string animation="idle"){
    sprite.Play(idle?"idle":animation);
  }

  public void MakeCameraCurrent()
  {
    camera.MakeCurrent();
  }
}