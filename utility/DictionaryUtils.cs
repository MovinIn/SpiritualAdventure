using System.Collections.Generic;

namespace SpiritualAdventure.utility;

public static class DictionaryUtils
{
  public static void AddRange<TK,TV>(this Dictionary<TK,TV> destination, Dictionary<TK,TV> from)
  {
    foreach (var item in from)
    {
      destination[item.Key] = item.Value;
    }
  }
}