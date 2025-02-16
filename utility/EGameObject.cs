using System;
using System.Text.RegularExpressions;

namespace SpiritualAdventure.utility;

public enum EGameObject
{
  SpeechLine,Npc
}

public static class EGameObjectHelper
{
  private static Regex nonAlphanumeric;

  static EGameObjectHelper()
  {
    nonAlphanumeric = new Regex(@"\W|_");
  }
  
  public static EGameObject Identify(string typeIdentifier)
  {
    string processedIdentifier = nonAlphanumeric.Replace(typeIdentifier.ToLower(), "");
      
    foreach (var eGameObject in Enum.GetValues<EGameObject>())
    {
      if (processedIdentifier.Equals(eGameObject.ToString().ToLower()))
      {
        return eGameObject;
      }
    }
    
    throw new FormatException("Invalid type identifier " + typeIdentifier);
  }
}
