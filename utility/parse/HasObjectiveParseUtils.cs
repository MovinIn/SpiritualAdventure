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

		return parser.DynamicParse<Objective,JObject>(token, null, 
		  o => ObjectiveParseUtils.Parse(o, parser));
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
	string[] incorrectOptions = ((JArray)dyn.incorrectOptions ?? new JArray())
	  .Children().Select(token => token.Value<string>()).ToArray();
	return new OptionObjective(correctOption,objective,incorrectOptions);
  }
  
  private static IHasObjective ParseSimpleCutscene(dynamic dyn, DynamicParser parser)
  {
	JArray tupleArray=dyn.actions;
	List<Tuple<SpeechAction,List<ICutsceneAction>>> actions=tupleArray.Children().Select(token =>
	{
	  return parser.DynamicParse<Tuple<SpeechAction,List<ICutsceneAction>>,JToken>(token,null,
		t=>ParseSimpleCutsceneObjectiveTuple(t,parser));
	}).ToList();

	return new SimpleCutsceneObjective(actions);
  }

  private static Tuple<SpeechAction,List<ICutsceneAction>> ParseSimpleCutsceneObjectiveTuple(
	JToken token,DynamicParser parser)
  {
	if (token.Type == JTokenType.Array)
	{
	  return SimpleCutsceneObjective.DelayedActionsWithoutSpeech(((JArray)token)[1].Value<float>());
	}

	dynamic tupleDyn = token.ToObject<JObject>();
	SpeechAction speechAction;
	if (tupleDyn.speechAction == null)
	{
	  speechAction = new SpeechAction(new Narrator(), null, 0);
	}
	else{
	  GD.Print(tupleDyn.speechAction);
	  speechAction = parser.DynamicParse<SpeechAction, JObject>((JToken)tupleDyn.speechAction,
		null,o  => CutsceneParseUtils.ParseSpeechAction(o, parser));
	}
	JArray actions=tupleDyn.actions;
	List<ICutsceneAction> cutsceneActions;
	if (actions == null)
	{
	  cutsceneActions = new List<ICutsceneAction>();
	}
	else
	{
	  cutsceneActions=actions.Children().Select(actionToken => CutsceneParseUtils.Parse(
		parser.OrOfPointer<JObject>(actionToken, null, out _), parser)).ToList();
	}

	return new Tuple<SpeechAction, List<ICutsceneAction>>(speechAction,cutsceneActions);
  }
  
  private static IHasObjective ParseTouch(dynamic dyn, DynamicParser parser)
  {
	JArray positionArr = dyn.position;
	var position = GameUnitUtils.Vector2(positionArr[0].Value<float>(), positionArr[1].Value<float>());
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
