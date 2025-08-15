using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.cutscene.actions;
using SpiritualAdventure.entities;
using static SpiritualAdventure.utility.parse.DynamicParserUtils;

namespace SpiritualAdventure.utility.parse;

public static class CutsceneParseUtils
{
  private delegate ICutsceneAction ParseCutsceneAction(JObject data,DynamicParser parser);
  
  private static readonly Dictionary<string, ParseCutsceneAction> cactionMap;
  
  static CutsceneParseUtils()
  {
    cactionMap = new Dictionary<string, ParseCutsceneAction>
    {
      {"cmovement",ParseMovement},
      {"pan",ParsePan},
      {"inline",ParseInline},
      {"flash",ParseFlash},
      {"speech",ParseSpeechAction}
    };
  }

  public static ICutsceneAction Parse(JObject data,DynamicParser parser)
  {
    string type = data.Value<string>("type");
    if (!cactionMap.TryGetValue(type, out var cactionParser))
    {
      throw new ArgumentException("got unparseable type <"+type+"> when trying to parse CutsceneAction.");
    }
    return cactionParser.Invoke(data,parser);
  }
  
  public static SpeechAction ParseSpeechAction(JObject data, DynamicParser parser)
  {
    dynamic dyn = data;
    var narrator=new Narrator(IdentityParseUtils.Parse(dyn));
    SpeechLine line = SpeechParseUtils.Parse(dyn.speech, parser);
    return new SpeechAction(narrator, line, 0);
  }
  
  public static InlineCutsceneAction ParseFlash(JObject data, DynamicParser _)
  {
    return FlashParseUtils.Parse(data["queue"].ToObject<JArray>());
  }
  
  /**
   * Creates an inline object to be modified inside _Ready() of Level[X].cs via the pointer address.
   */
  public static InlineCutsceneAction ParseInline(JObject data, DynamicParser parser)
  {
    string pointer=data.Value<string>("pointer");
    GD.Print("ParseInline: "+pointer);
    var inline=new InlineCutsceneAction(() => { });
    parser.filteredPointers[pointer] = inline;
    return inline;
  }

  public static PanCutsceneAction ParsePan(JObject data, DynamicParser parser)
  {
    dynamic dyn = data;
    JArray positionArr = dyn.position;
    var position = GameUnitUtils.Vector2(positionArr[0].Value<float>(), positionArr[1].Value<float>());
    return new PanCutsceneAction(position);
  }

  public static MovementCutsceneAction ParseMovement(JObject data, DynamicParser parser)
  {
    dynamic dyn = data;
    var npc=(PathDeterminantNpc) DynamicParseNpc(dyn.npc, parser);
    List<MovementAction> moves=MovementActionParseUtils.Parse(dyn.moves);
    float moveDelay = dyn.moveDelay ?? 0;
    bool isRelativePath = dyn.relativePath ?? true;
    bool repeatMotion = dyn.repeatMotion ?? false;
    return new MovementCutsceneAction(npc, moves, moveDelay, isRelativePath, repeatMotion,0);
  }
}