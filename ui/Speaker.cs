using Godot;

namespace SpiritualAdventure.ui;

public enum Speaker
{
    Archer,
    Archer2,
    
    Cowboy,
    Cowboy2,
    
    Layman,
    Layman2,
    
    Black_Cowboy,
    Black_Cowboy2,
    
    Swordsman,
    Swordsman2,
    
    Red_Warrior,
    Red_Warrior2,
    
    Red_Merchant,
    Red_Merchant2,
    
    Black_Merchant,
    Black_Merchant2,
}

internal static class SpeakerExtensions
{
    public static Texture2D asTexture(this Speaker speaker)
    {
        return ResourceLoader.Load<Texture2D>("res://assets/"+speaker.ToString().Replace("_","").ToLower()+".png");
    }

    public static SpriteFrames asFrames(this Speaker speaker)
    {
        return ResourceLoader.Load<SpriteFrames>("res://assets/frames/" + 
                                                 speaker.ToString().Replace("_", "").ToLower() + ".tres");
    }

    public static string asTitle(this Speaker speaker)
    {
        return speaker.ToString().Replace("_", " ").Replace("2","");
    }
}