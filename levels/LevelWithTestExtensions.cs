using System;
using System.Collections.Generic;
using Godot;
using SpiritualAdventure.cutscene.actions;
using SpiritualAdventure.entities;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.levels;

public partial class LevelWithTestExtensions:Level
{
  protected ObjectiveDisplayGroup TestTouchObjective(Vector2 position,int timeLimit=-1)
  {
    var touchObjective=TouchObjective.Instantiate(new Objective("Touch the Checkpoint at "+position));
    touchObjective.Position = position;
    AddChild(touchObjective);

    return new ObjectiveDisplayGroup(new List<IHasObjective> { touchObjective }, timeLimit);
  }
  
  protected ObjectiveDisplayGroup CutsceneTest(PathDeterminantNpc npc)
  {
    var redWarriorIdentity = new Identity(Speaker.Red_Warrior,"Solomon");
    SimpleCutsceneObjective cutsceneObjective=new(
      new List<Tuple<SpeechAction, List<ICutsceneAction>>> {
        
        SimpleCutsceneObjective.DelayedActionGroupWithoutSpeech(0,new List<ICutsceneAction>
        {
          new PanCutsceneAction(new Vector2(1000, 1000))
        }),
        
        new(new SpeechAction(narrator,new SpeechLine(redWarriorIdentity,"I can't believe I'm going to walk right..."),1),
          new List<ICutsceneAction>
          {
            new MovementCutsceneAction(npc,new List<MovementAction>{new (100,0)},0,true,false,1.5)
          }),
        
        new(new SpeechAction(narrator,new SpeechLine(redWarriorIdentity,"Just to walk left again."),3),
          new List<ICutsceneAction>
          {
            new MovementCutsceneAction(npc,new List<MovementAction>{new (-100,0)},0,true,false,1.5)
          }),
        
        new(new SpeechAction(narrator,new SpeechLine(redWarriorIdentity,"How life is meaningless without God."),3),
          new List<ICutsceneAction>()),
        
        SimpleCutsceneObjective.DelayedActionGroupWithoutSpeech(1)
        
      });
    return new ObjectiveDisplayGroup(new List<IHasObjective>{cutsceneObjective});
  }
  
  public ObjectiveDisplayGroup TestChatObjective(Vector2 position,int timeLimit=-1)
  {
    var npc=Npc.Instantiate().WithPosition(position);
    AddChild(npc);
    
    var speechLines = new List<SpeechLine>
    { 
      new(Narrator.Identity,"This should have completed the Objective!")
    };
    
    npc.Who(Speaker.Red_Warrior,"Roman");
    npc.UseTrigger("interact","Talk");
    npc.SetSpeech(speechLines);
    var objective = new Objective("Talk to the NPC at (150,150)", 
      new SpeechLine(Narrator.Identity,"This is a MULTI-LINE Post Completion Feedback Line!")
        .SetNext(new SpeechLine(Narrator.Identity,"A special treatment for NPCs!")));
    
    StartChatObjective c=new(npc,objective);
    
    return new ObjectiveDisplayGroup(new List<IHasObjective>{c},timeLimit);
  }
  
  public ObjectiveDisplayGroup TestOptionObjective(Vector2 position,int timeLimit=-1)
  {
    var npc=Npc.Instantiate().WithPosition(position);
    var npcIdentity = new Identity(Speaker.Red_Warrior,"Roman");
    AddChild(npc);
    var speechLines = new List<SpeechLine>
    { 
      new SpeechLine(npcIdentity,"Option 2 is the only correct answer and should complete the objective. Option 3 should fail the objective.").SetOptions(
        new Dictionary<string, SpeechLine>
        {
          {"Option 1",new SpeechLine(npcIdentity,"You chose Option 1. Now what?")},
          {"Option 2",new SpeechLine(npcIdentity,"You chose Option 2! This should have completed the Objective.")},
          {"Option 3",new SpeechLine(npcIdentity,"You chose Option 3. This should have failed the Objective.")}
        })
    };
    npc.Who(Speaker.Red_Warrior,"Roman");
    npc.UseTrigger("interact","Talk");
    npc.SetSpeech(speechLines);
    var optionObjective = new OptionObjective("Option 2",new Objective("Choose the correct objective"),
      new[]{"Option 3"});
    
    return new ObjectiveDisplayGroup(new List<IHasObjective>{optionObjective},timeLimit);
  }
}