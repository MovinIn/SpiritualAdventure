using Godot;

namespace SpiritualAdventure.ui;

public partial class Root : Node
{
	public static Player player;
	public override void _Ready()
	{
		player= GetNode<Player>("Background/Player");
	}
}
