using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json.Linq;
using SpiritualAdventure.cutscene.actions;
using SpiritualAdventure.entities;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;
using SpiritualAdventure.utility.parse;
using static SpiritualAdventure.objects.SimpleCutsceneObjective;

namespace SpiritualAdventure.levels;

public partial class Level8:Level
{
  public override void _Ready()
  {
    //Space out npcs
    string[] npcPointers = { "1","2","3","4","5","6","7","8"};
    Npc[] npcs=new Npc[npcPointers.Length];
    for (int i = 0; i < npcs.Length; i++)
    {
      npcs[i]=(Npc) builder.parser.filteredPointers[npcPointers[i]];
      npcs[i].Position = new Vector2((i+1)*600,300*(i%2-1));
    }

    Npc herod = (Npc)builder.parser.filteredPointers["herod"];
    FinishChatObjective h1 = new FinishChatObjective(herod,new Objective("Discover the birthplace of the coming Messiah, Jesus Christ."));
    IHasObjective c1 = HerodCutsceneObjective();
    TouchObjective sleepObjective = TouchObjective.Instantiate(new Objective("Sleep back at home"));
    sleepObjective.Position = new Vector2(6,6);
    IHasObjective c2 = DreamCutsceneObjective();
    Npc joseph = (Npc)builder.parser.filteredPointers["joseph"];
    StartChatObjective j1 = new StartChatObjective(joseph,
      new Objective("Give gifts to Jesus via Joseph",DynamicParseSpeech("end")));

    var objectiveBuilder = LevelBuilder.Init()
      .AppendIObjectiveGroups(new List<ObjectiveDisplayGroup>
      {
        new(new List<IHasObjective>
        {
          h1,c1,sleepObjective,c2,j1
        })
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
        DelayedActionsWithoutSpeech(1.5f,new List<ICutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.ToColor(Colors.Black,3);
            Flash.Initiate();
          }),
          new PanCutsceneAction(c1Cam),
        }),
        DelayedActionsWithoutSpeech(4,new List<ICutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.Dissolve(3);
            Flash.Initiate();
          }),
        }),
        DelayedActionsWithoutSpeech(4),
        new(new SpeechAction(new Narrator(),DynamicParseSpeech("h2"),1),
          new List<ICutsceneAction>()),
        DelayedActionsWithoutSpeech(1.5f,new List<ICutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.ToColor(Colors.Black,3);
            Flash.Initiate();
          })
        }),
        DelayedActionsWithoutSpeech(4,new List<ICutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            player.Position = sleepCam;
            player.MakeCameraCurrent();
            Flash.Dissolve(1);
            Flash.Initiate();
          })
        })
      });

    c1.objective.postCompletionFeedback = new SpeechLine("Great work discovering the birthplace of the Messiah! " +
                                                         "Now go back to bed and carry out Herod's orders tomorrow.");
    return c1;
  }

  private IHasObjective DreamCutsceneObjective()
  {
    Vector2 c2Cam = new Vector2();
    
    
    var c2 = new SimpleCutsceneObjective(
      new List<Tuple<SpeechAction, List<ICutsceneAction>>>
      {
        DelayedActionsWithoutSpeech(0,new List<ICutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.ToColor(Colors.Black,3);
            Flash.Initiate();
          }),
        }),
        DelayedActionsWithoutSpeech(4,new List<ICutsceneAction>
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
        DelayedActionsWithoutSpeech(2,new List<ICutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.Dissolve(2);
            Flash.Initiate();
          }),
          new PanCutsceneAction(c2Cam),
        }),
        DelayedActionsWithoutSpeech(2)
      });
    c2.objective.postCompletionFeedback = new SpeechLine("Good work! Now impart your gifts to the Messiah, Jesus Christ.");
    return c2;
  }

  //TODO: move to Level.cs and add dynamicparsing of npc as well.
  private SpeechLine DynamicParseSpeech(string pointer)
  {
    return builder.parser.DynamicParse<SpeechLine, JObject>(pointer, null,
      o => SpeechParseUtils.Parse(o,builder.parser));
  }
}