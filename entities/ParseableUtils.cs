using Newtonsoft.Json.Linq;

namespace SpiritualAdventure.entities;

public class ParseableUtils
{
  public delegate bool TryOfPointer(string pointer,out JToken unparsed);
}