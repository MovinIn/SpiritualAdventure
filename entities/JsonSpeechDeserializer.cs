using System.Collections.Generic;
using Newtonsoft.Json;

namespace SpiritualAdventure.entities;

public static class JsonSpeechDeserializer
{
    public static string Serialize(List<SpeechLine> speechLines)
    {
        return JsonConvert.SerializeObject(speechLines);
    }

    public static List<SpeechLine> Deserialize(string json)
    {
      return JsonConvert.DeserializeObject<List<SpeechLine>>(json);
    }
}