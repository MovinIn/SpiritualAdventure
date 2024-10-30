using Godot;
using System;
using SpiritualAdventure.ui;

public partial class SpeakerDetails : HBoxContainer
{
    private TextureRect avatar;
    RichTextLabel name;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        avatar=GetNode<TextureRect>("avatar");
        name=GetNode<RichTextLabel>("VBoxContainer/name");
	}

    public void setSpeaker(Speaker speaker)
    {
        setSpeaker(ResourceLoader.Load<Texture2D>(speaker.asPath()),speaker.asTitle());
    }
    public void setSpeaker(Speaker speaker,string name)
    {
        setSpeaker(ResourceLoader.Load<Texture2D>(speaker.asPath()),name);
    }

    public void setSpeaker(Texture2D texture,string name)
    {
        avatar.SetTexture(texture);
        this.name.Text = name;
    }
}
