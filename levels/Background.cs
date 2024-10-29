using System.Collections.Generic;
using Godot;
using SpiritualAdventure.entities;

public partial class Background : Node2D
{
	Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player=GetNode<Player>("Player");
        testNPC(new Vector2(200,200));
    }

    public void testNPC(Vector2 position)
    {
        Npc npc=PathDeterminantNpc.instance(
            new List<Vector2>{new (0,0),new (100,0),new (100,100),new (0,100),new (0,0)},
            position,2,true);
        AddChild(npc);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
