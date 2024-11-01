using Godot;
using System;
using SpiritualAdventure.ui;

public partial class SpeakerDetails : GridContainer
{
    private TextureRect avatar;
    RichTextLabel name;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        avatar=GetNode<TextureRect>("%Avatar");
        name=GetNode<RichTextLabel>("%Name");
	}

    public void setSpeaker(Speaker speaker)
    {
        setSpeaker(speaker.asTexture(),speaker.asTitle());
    }
    public void setSpeaker(Speaker speaker,string name)
    {
        setSpeaker(speaker.asTexture(),name);
    }

    public void setSpeaker(Texture2D texture,string name)
    {
        avatar.SetTexture(texture);
        this.name.Text = name;
    }
}
