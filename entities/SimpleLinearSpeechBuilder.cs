using System.Collections.Generic;

namespace SpiritualAdventure.entities;

public static class SimpleLinearSpeechBuilder
{
    public static SpeechLine Of(List<string> lines)
    {
        if (lines.Count == 0)
        {
            return null;
        }
        var head=new SpeechLine(lines[0]);
        SpeechLine s = head;
        SpeechLine prev=s;
        for (int i = 1; i < lines.Count; i++)
        { 
            s = new SpeechLine(lines[i]);
            prev.next = s;
            prev = s;
        }

        return head;
    }
}