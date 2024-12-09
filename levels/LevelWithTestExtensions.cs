using System;
using System.Collections.Generic;
using Godot;
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
    var redWarrior = new Narrator(Speaker.Red_Warrior,"Solomon");
    SimpleCutsceneObjective cutsceneObjective=new(
      new List<Tuple<SpeechAction, List<CutsceneAction>>> {
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(0,new List<CutsceneAction>
        {
          new PanCutsceneAction(new Vector2(1000, 1000))
        }),
        
        new(new SpeechAction(redWarrior,new SpeechLine("I can't believe I'm going to walk right..."),1),
          new List<CutsceneAction>
          {
            new CutsceneMovementAction(npc,new List<MovementAction>{new (100,0)},0,true,false,1.5)
          }),
        
        new(new SpeechAction(redWarrior,new SpeechLine("Just to walk left again."),3),
          new List<CutsceneAction>
          {
            new CutsceneMovementAction(npc,new List<MovementAction>{new (-100,0)},0,true,false,1.5)
          }),
        
        new(new SpeechAction(redWarrior,new SpeechLine("How life is meaningless without God."),3),
          new List<CutsceneAction>()),
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(1)
        
      });
    return new ObjectiveDisplayGroup(new List<IHasObjective>{cutsceneObjective});
  }
  
  public ObjectiveDisplayGroup TestChatObjective(Vector2 position,int timeLimit=-1)
  {
    var npc=Npc.Instantiate(position);
    AddChild(npc);
    
    var speechLines = new List<SpeechLine>
    { 
      new("This should have completed the Objective!")
    };
    
    npc.Who(Speaker.Red_Warrior,"Roman");
    npc.UseTrigger("interact","Talk");
    npc.SetSpeech(speechLines);
    var objective = new Objective("Talk to the NPC at (150,150)", 
      new SpeechLine("This is a MULTI-LINE Post Completion Feedback Line!",
      new SpeechLine("A special treatment for NPCs!")));
    
    StartChatObjective c=new(npc,objective);
    
    return new ObjectiveDisplayGroup(new List<IHasObjective>{c},timeLimit);
  }
  
  public ObjectiveDisplayGroup TestOptionObjective(Vector2 position,int timeLimit=-1)
  {
    var npc=Npc.Instantiate(position);
    AddChild(npc);
    var speechLines = new List<SpeechLine>
    { 
      new("Option 2 is the only correct answer and should complete the objective. Option 3 should fail the objective.",
        new Dictionary<string, SpeechLine>
        {
          {"Option 1",new SpeechLine("You chose Option 1. Now what?")},
          {"Option 2",new SpeechLine("You chose Option 2! This should have completed the Objective.")},
          {"Option 3",new SpeechLine("You chose Option 3. This should have failed the Objective.")}
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