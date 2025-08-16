using System.Collections.Generic;
using SpiritualAdventure.objectives;

namespace SpiritualAdventure.entities;

public static class SimpleLinearSpeechBuilder
{
    public static SpeechLine Of(Identity identity,List<string> lines)
    {
        if (lines.Count == 0)
        {
            return null;
        }

        var head = new SpeechLine(identity,lines[0]);
        SpeechLine s = head;
        SpeechLine prev=s;
        for (int i = 1; i < lines.Count; i++)
        {
          s = new SpeechLine(identity,lines[i]);
            prev.next = s;
            prev = s;
        }

        return head;
    }
}