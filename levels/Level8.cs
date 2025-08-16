using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.cutscene.actions;
using SpiritualAdventure.entities;
using SpiritualAdventure.objectives;
using SpiritualAdventure.utility.parse;
using static SpiritualAdventure.objectives.SimpleCutsceneObjective;

namespace SpiritualAdventure.levels;

public partial class Level8:Level
{
  public override void _Ready()
  {
    GD.Print(builder.parser.filteredPointers.Count);
    foreach (var pair in builder.parser.filteredPointers)
    {
      GD.Print(pair.Key+","+pair.Value);
    }
    
    //Space out npcs
    string[] npcPointers = {"1","2","3","4","5","6","7","8"};
    for (int i = 0; i < npcPointers.Length; i++)
    {
      Npc npc=(Npc) builder.parser.filteredPointers[npcPointers[i]];
      npc.Position = new Vector2((i+1)*200,300*(i%2)-150);
    }

    Npc herod = (Npc)builder.parser.filteredPointers["herod"];
    FinishChatObjective h1 = new FinishChatObjective(herod,new Objective("Discover the birthplace of the coming Messiah, Jesus Christ."));
    IHasObjective c1 = HerodCutsceneObjective();
    TouchObjective sleepObjective = TouchObjective.Instantiate(new Objective("Sleep back at home"));
    sleepObjective.Position = new Vector2(200,200);
    AddChild(sleepObjective);
    IHasObjective c2 = DreamCutsceneObjective();
    Npc joseph = (Npc)builder.parser.filteredPointers["joseph"];
    StartChatObjective j1 = new StartChatObjective(joseph,
      new Objective("Give gifts to Jesus via Joseph",DynamicParseSpeech("end")));

    var objectiveBuilder = LevelBuilder.Init()
      .AppendIObjectiveGroups(new List<ObjectiveDisplayGroup>
      {
        new(new List<IHasObjective> {h1}),
        new(new List<IHasObjective> {c1}),
        new(new List<IHasObjective> {sleepObjective}),
        new(new List<IHasObjective> {c2}),
        new(new List<IHasObjective> {j1})
      });
	
    AppendBuilder(objectiveBuilder);
    LoadLevel();
    
    NextObjective();
  }

  private IHasObjective HerodCutsceneObjective()
  {
    Vector2 c1Cam = new Vector2();
    Vector2 sleepCam = new Vector2();
	
    SimpleCutsceneObjective c1 = new SimpleCutsceneObjective(
      new List<Tuple<SpeechAction, List<ICutsceneAction>>>
      {
        DelayedActionGroupWithoutSpeech(1.5f,new List<ICutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.ToColor(Colors.Black,3);
            Flash.Initiate();
          }),
        }),
        DelayedActionGroupWithoutSpeech(4,new List<ICutsceneAction>
        {
          new PanCutsceneAction(c1Cam),
          new InlineCutsceneAction(() =>
          {
            player.Position = sleepCam;
            Flash.Dissolve(3);
            Flash.Initiate();
          })
        }),
        DelayedActionGroupWithoutSpeech(4),
        new(new SpeechAction(new Narrator(),DynamicParseSpeech("h2"),1),
          new List<ICutsceneAction>()),
        DelayedActionGroupWithoutSpeech(1.5f,new List<ICutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.ToColor(Colors.Black,3);
            Flash.Initiate();
          })
        }),
        DelayedActionGroupWithoutSpeech(4,new List<ICutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            player.Position = sleepCam;
            player.MakeCameraCurrent();
            Flash.Dissolve(1);
            Flash.Initiate();
          })
        }),
        DelayedActionGroupWithoutSpeech(2,new List<ICutsceneAction>())
      });

    c1.objective.postCompletionFeedback = new SpeechLine(Narrator.Identity,"Great work discovering the birthplace of the Messiah! " +
                                                         "Now go back to bed and carry out Herod's orders tomorrow.");
    return c1;
  }

  private IHasObjective DreamCutsceneObjective()
  {
    Vector2 c2Cam = new Vector2();
	
	
    var c2 = new SimpleCutsceneObjective(
      new List<Tuple<SpeechAction, List<ICutsceneAction>>>
      {
        DelayedActionGroupWithoutSpeech(0,new List<ICutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.ToColor(Colors.Black,3);
            Flash.Initiate();
          }),
        }),
        DelayedActionGroupWithoutSpeech(4,new List<ICutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.ToColor(Colors.White,1);
            Flash.Initiate();
          }),
        }),
        new(new SpeechAction(new Narrator(),DynamicParseSpeech("warning1"),1),
          new List<ICutsceneAction>()),
        new(new SpeechAction(new Narrator(),DynamicParseSpeech("warning2"),1),
          new List<ICutsceneAction>()),
        DelayedActionGroupWithoutSpeech(2,new List<ICutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.Dissolve(2);
            Flash.Initiate();
          }),
          new PanCutsceneAction(c2Cam),
        }),
        DelayedActionGroupWithoutSpeech(2)
      });
    c2.objective.postCompletionFeedback = new SpeechLine(Narrator.Identity,"Good work! Now impart your gifts to the Messiah, Jesus Christ.");
    return c2;
  }

  //TODO: move to Level.cs and add dynamicparsing of npc as well.
  private SpeechLine DynamicParseSpeech(string pointer)
  {
    return builder.parser.DynamicParse<SpeechLine, JObject>(pointer, null,
      o => SpeechParseUtils.Parse(o,builder.parser));
  }
}