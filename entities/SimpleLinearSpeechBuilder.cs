using System.Collections.Generic;

namespace SpiritualAdventure.entities;

public static class SimpleLinearSpeechBuilder
{
    public static SpeechLine of(List<string> lines)
    {
        if (lines.Count == 0)
        {
            return null;
        }
        SpeechLine head=new SpeechLine(lines[0],(SpeechLine)null);
        SpeechLine s = head;
        SpeechLine prev=s;
        for (int i = 1; i < lines.Count; i++)
        { 
            s = new SpeechLine(lines[i],(SpeechLine)null);
            prev.next = s;
            prev = s;
        }

        return head;
    }
}