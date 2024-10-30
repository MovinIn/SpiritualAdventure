using Godot;
using System;

public partial class InteractTriggerDisplay : RichTextLabel
{
    private char key;
    private string content;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        key = 'Q';
        Visible = false;
    }

    public void set(char key,string content)
    {
        this.key = Char.ToUpper(key);
        this.content = content;
    }

    public void show()
    {
        if (String.IsNullOrEmpty(content)) return;

        Text = "[center]"+"[" + key + "] " + content+"[/center]";
        Visible = true;
    }

    public void hide()
    {
        Visible = false;
    }
}
