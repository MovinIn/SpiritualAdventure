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

static class SpeakerExtensions
{
    public static string asPath(this Speaker speaker)
    {
        return "res://assets/"+speaker.ToString().Replace("_","").ToLower()+".png";
    }

    public static string asTitle(this Speaker speaker)
    {
        return speaker.ToString().Replace("_", " ");
    }
}