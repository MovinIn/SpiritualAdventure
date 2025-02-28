using Godot;
using System;
using SpiritualAdventure.objectives;
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

    public void SetIdentity(Identity identity)
    {
      SetIdentity(identity.speaker,identity.name);
    }
    
    public void SetIdentity(Speaker speaker)
    {
        SetIdentity(speaker.asTexture(),speaker.asTitle());
    }
    public void SetIdentity(Speaker speaker,string name)
    {
        SetIdentity(speaker.asTexture(),name);
    }

    public void SetIdentity(Texture2D texture,string name)
    {
        avatar.SetTexture(texture);
        this.name.Text = name;
    }
}
