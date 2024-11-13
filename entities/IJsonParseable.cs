using Newtonsoft.Json.Linq;

namespace SpiritualAdventure.entities;

public interface IJsonParseable
{
  public void Parse(JObject json);
}