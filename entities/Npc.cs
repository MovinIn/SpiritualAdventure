using Godot;
using System;
using System.Collections.Generic;
using SpiritualAdventure.entities;

public partial class Npc : AnimatableBody2D
{

    [Signal]
    public delegate void interactEventHandler();
    
	protected const float SPEED = 120f;
	protected CharacterSprite sprite;
    protected Queue<string> speech;

    protected Npc()
    {
        speech=new Queue<string>();
    }
    
	public override void _Ready()
	{
		sprite=GetNode<CharacterSprite>("Sprite");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		sprite.updateRotation(ConstantLinearVelocity.X);
	}

    public virtual void onInteractBodyEntered(Node2D body) {}

    public string nextLine()
    {
        if (speech.Count == 0) return null;
        string line = speech.Dequeue();
        speech.Enqueue(line);
        return line;
    }
}
