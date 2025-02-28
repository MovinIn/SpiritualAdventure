using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.cutscene.actions;
using SpiritualAdventure.entities;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;
using static SpiritualAdventure.utility.parse.DynamicParserUtils;

namespace SpiritualAdventure.utility.parse;

public static class HasObjectiveParseUtils
{
  private delegate IHasObjective ParseIHasObjective(JObject data,DynamicParser parser);

  private static readonly Dictionary<string, ParseIHasObjective> ihoMap;

  static HasObjectiveParseUtils()
  {
    ihoMap = new Dictionary<string, ParseIHasObjective>
    {
      {"finishChatObjective",ParseFinishChat},
      {"negative",ParseNegative},
      {"option",ParseOption},
      {"cutscene",ParseSimpleCutscene},
      {"startChat",ParseStartChat},
      {"targetSpeech",ParseTargetSpeech},
      {"touch",ParseTouch}
    };
  }

  public static IHasObjective Parse(JObject data,DynamicParser parser)
  {
    string type = data.Value<string>("type");
    if (!ihoMap.TryGetValue(type, out var ihoParser))
    {
      throw new ArgumentException("got unparseable type <"+type+"> when trying to parse IHasObjective.");
    }
    return ihoParser.Invoke(data,parser);
  }

  private static IHasObjective ParseNegative(dynamic dyn, DynamicParser parser)
  {
    Objective objective = DynamicCloneObjective(dyn.objective,parser);
    
    List<Objective> negativeObjectives = ((JArray)dyn.negativeObjectives ?? new JArray())
      .Children().Select(token =>
      {
        if (token.Type != JTokenType.String)
        {
          throw new ArgumentException("Tried to parse NegativeObjective, but one or more negativeObjectives" +
                                      " was not a pointer, but of unparsable type: <"+token.Type+">.");
        }

        string type = token.Value<string>();
      
        if (!parser.filteredPointers.TryGetValue(type, out var pointer))
        {
          throw new ArgumentException("Tried to parse NegativeObjective, but one or more negativeObjectives" +
                                      " was not a pointer, ie. <"+token.Value<string>()+">.");
        }

        if (pointer is not Objective value)
        {
          throw new ArgumentException("Tried to parse NegativeObjective, but one or more negativeObjectives" +
                                      " pointed to an object not of type Objective," +
                                      " but of type <"+pointer.GetType()+">.");
        }
      
        return value;
      }).ToList();

    return new NegativeObjective(objective, negativeObjectives);
  }

  private static IHasObjective ParseFinishChat(dynamic dyn,DynamicParser parser)
  {
    Npc npc = DynamicParseNpc(dyn.npc,parser);
    Objective objective = DynamicCloneObjective(dyn.objective,parser);
    return new FinishChatObjective(npc, objective);
  }
  
  private static IHasObjective ParseOption(dynamic dyn, DynamicParser parser)
  {
    string correctOption = dyn.correctOption;
    Objective objective = DynamicCloneObjective(dyn.objective,parser);
    string[] incorrectOptions = ((JArray)dyn.negativeObjectives ?? new JArray())
      .Children().Select(token => token.Value<string>()).ToArray();
    return new OptionObjective(correctOption,objective,incorrectOptions);
  }
  
  private static IHasObjective ParseSimpleCutscene(dynamic dyn, DynamicParser parser)
  {
    JArray tupleArray=dyn.actions;
    List<Tuple<SpeechAction,List<ICutsceneAction>>> actions=tupleArray.Children().Select(token =>
    {
      dynamic tupleDyn = token.ToObject<JObject>();
      
      SpeechAction speechAction = parser.DynamicParse<SpeechAction, JObject>(tupleDyn.speechAction,
        null,new Func<JObject, SpeechAction>(o  => CutsceneParseUtils.ParseSpeechAction(o, parser)));
      JArray actions=tupleDyn.actions;
      List<ICutsceneAction> cutsceneActions=actions.Children().Select(actionToken => CutsceneParseUtils.Parse(
        parser.OrOfPointer<JObject>(actionToken, null, out _), parser)).ToList();
      return new Tuple<SpeechAction, List<ICutsceneAction>>(speechAction,cutsceneActions);
    }).ToList();

    return new SimpleCutsceneObjective(actions);
  }
  
  private static IHasObjective ParseTouch(dynamic dyn, DynamicParser parser)
  {
    JArray positionArr = dyn.position;
    var position = new Vector2(positionArr[0].Value<float>(), positionArr[1].Value<float>());
    Objective objective = DynamicCloneObjective(dyn.objective,parser);
    var touchObjective = TouchObjective.Instantiate(objective);
    touchObjective.Position = position;
    return touchObjective;
  }

  private static IHasObjective ParseTargetSpeech(dynamic dyn, DynamicParser parser)
  {
    string targetLine = dyn.target;
    Objective objective=DynamicCloneObjective(dyn.objective,parser);
    return new TargetSpeechObjective(targetLine, objective);
  }

  private static IHasObjective ParseStartChat(dynamic dyn, DynamicParser parser)
  {
    Npc npc = DynamicParseNpc(dyn.npc,parser);
    Objective objective=DynamicCloneObjective(dyn.objective,parser);
    return new StartChatObjective(npc, objective);
  }
}