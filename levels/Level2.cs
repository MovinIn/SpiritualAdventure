using System;
using Godot;
using System.Collections.Generic;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

public partial class Level2 : Level
{
  public override void _Ready()
  {
    var npc = PathDeterminantNpc.Instantiate(new List<MovementAction>(),new Vector2(1050,1050),
      0,true,false);
    
    var touchObjective= TestTouchObjective(new Vector2(200,200));
    var cutsceneObjective = CutsceneTest(npc);
    LoadLevel(new Vector2(0,0),new List<IHasObjective>{touchObjective,cutsceneObjective},
      new List<Npc>{npc},new Narrator());
    NextObjective();
  }

  public IHasObjective TestTouchObjective(Vector2 position,int timeLimit=-1)
  {
    var touchObjective=TouchObjective.Instantiate(ObjectiveBuilder.TimedOrElse(
      "Touch the Checkpoint at "+position,timeLimit));
    touchObjective.Position = position;
    AddChild(touchObjective);
    return touchObjective;
  }

  public IHasObjective CutsceneTest(PathDeterminantNpc npc)
  {
    var redWarrior = new Narrator(Speaker.Red_Warrior,"Solomon");
    SimpleCutsceneObjective cutsceneObjective=new(
      new List<Tuple<SpeechAction, List<CutsceneAction>>> {
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
        new(new SpeechAction(redWarrior,null,1),
          new List<CutsceneAction>())
      }, new Vector2(1000,1000));
    return cutsceneObjective;
  }
}