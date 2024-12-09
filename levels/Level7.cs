using Godot;
using System;
using System.Collections.Generic;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

public partial class Level7 : Level
{
  private TileMapLayer crossLayer,beforeCrossLayer,red1,red2,red3;
  
  
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    crossLayer = GetNode<TileMapLayer>("CrossLayer");
    beforeCrossLayer = GetNode<TileMapLayer>("BeforeCrossLayer");
    red1 = GetNode<TileMapLayer>("Red1");
    red2 = GetNode<TileMapLayer>("Red2");
    red3 = GetNode<TileMapLayer>("Red3");

    red1.Visible = false;
    red2.Visible = false;
    red3.Visible = false;
    crossLayer.Visible = false;

    LoadLevel(new Vector2(1000,1000),
      new List<ObjectiveDisplayGroup>{CourtCutsceneObjective(),JesusTortureCutsceneObjective()},
      new List<Npc>(),new Narrator());
    NextObjective();
      
  }
    
  // (-1565.3118, 42.673996): cutscene cam position
  // (-1267.929, 162.674): right most judge position
  // (442.67752, -316.67258) cross cutscene position
  
    
  public ObjectiveDisplayGroup CourtCutsceneObjective()
  {

    var camPosition = new Vector2(-1565f, 42f);

    var rightJudgePosition = new Vector2(-1267, 162);

    var evilJudgeNarrator = new Narrator(Speaker.Red_Cowboy, "Evil Judge");
    
    for (int i = 0; i < 6; i++)
    {
      Npc npc = Npc.Instantiate(rightJudgePosition - i * new Vector2(75, 0));
      npc.Who(Speaker.Red_Cowboy,"");
      AddChild(npc);
    } 
    
    var narrator = new Narrator(Speaker.Archer,"Narrator");
    SimpleCutsceneObjective cutsceneObjective=new(
      new List<Tuple<SpeechAction, List<CutsceneAction>>> {
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(0,new List<CutsceneAction>
        {
          new PanCutsceneAction(camPosition)
        }),
        
        new(new SpeechAction(narrator,new SpeechLine("Using their high political power, they lied to charge Jesus" +
                                                     " of sedition in a kangaroo court. He was arrested and " +
                                                     "forced up a hill wearing a crown of thorns while bearing " +
                                                     "the cross."),1),
          new List<CutsceneAction>()),
        
        new(new SpeechAction(evilJudgeNarrator,new SpeechLine("Here, here! We have unanimously found Jesus " +
                                                             "Christ to be guilty of sedition!" ),1.5f),
          new List<CutsceneAction>()),
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(1,new List<CutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.ToColor(new Color(0,0,0,0),0);
            Flash.ToColor(Colors.Red,0.3f);
            Flash.Initiate();
          })
        }),
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(0.3f)
      });


    return new ObjectiveDisplayGroup(new List<IHasObjective> { cutsceneObjective });
  }

  public ObjectiveDisplayGroup JesusTortureCutsceneObjective()
  {
    var camPosition = new Vector2(442f, -316f);
    var initialJesusPosition=new Vector2(-87f, -7f);
    var initialGuardPosition = new Vector2(-218f, 55f);
    var initialGuardPosition2 = new Vector2(-223f,86f);

    var guardDistance = initialGuardPosition - initialGuardPosition2;
    
    
    var j1 = new Vector2(86,-101);
    var j2 = new Vector2(349,-219);
    var j3 = new Vector2(641,-236);

    PathDeterminantNpc jesus = PathDeterminantNpc.Instantiate(new List<MovementAction>(),initialJesusPosition,0,
      true,false);
    PathDeterminantNpc guard1 = PathDeterminantNpc.Instantiate(new List<MovementAction>(),initialGuardPosition,0,
      true,false);
    PathDeterminantNpc guard2 = PathDeterminantNpc.Instantiate(new List<MovementAction>(),initialGuardPosition2,0,
      true,false);
    
    jesus.Who(Speaker.Layman,"Jesus");
    guard1.Who(Speaker.Red_Warrior,"Guard 1");
    guard2.Who(Speaker.Red_Warrior,"Guard 2");
    
    AddChild(jesus);
    AddChild(guard1);
    AddChild(guard2);

    var guardNarrator = new Narrator(Speaker.Red_Warrior, "Soldier");
    var narrator = new Narrator(Speaker.Archer,"Narrator");

    var failString="Yeah I know. It's tempting to use power when an opportunity presents itself. " +
                      "But Jesus's plan is selfless, dying on the cross to pay for the penalty of our sins.";

    var targetSpeechObjective = new TargetSpeechObjective(new Objective(""),failString);
    
    var negativeObjective = new NegativeObjective(new Objective("Do not be tempted"),
      new List<Objective>{targetSpeechObjective.objective});
    
    
    SimpleCutsceneObjective cutsceneObjective=new(
      new List<Tuple<SpeechAction, List<CutsceneAction>>> {
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(0,new List<CutsceneAction>
        {
          new PanCutsceneAction(camPosition),
          new InlineCutsceneAction(() =>
          {
            Flash.Dissolve(2.5);
            Flash.Initiate();
          })
        }),
        
        new(new SpeechAction(guardNarrator,new SpeechLine("Move it! Why don't you save yourself if you are the Son of God?",
            new Dictionary<string, SpeechLine>
            {
              { "Shut up the Guard and free yourself through God's power", 
                new SpeechLine(failString)
              },
              {
                "Continue",
                new SpeechLine("Hahaha. Keep walking, you blasphemer.")
              }
            }),4),
          new List<CutsceneAction>
          {
            new CutsceneMovementAction(jesus,new List<MovementAction>
              {
                new(j1-jesus.Position)
              },
              0,true,false,0),
            new CutsceneMovementAction(guard1,new List<MovementAction>
              {
                new(initialJesusPosition-guard1.Position)
              },
              0,true,false,0),
            new CutsceneMovementAction(guard2,new List<MovementAction>
              {
                new(initialJesusPosition-guard1.Position-guardDistance)
              },
              0,true,false,0),
          }),
        
        FlashRed(),
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(0.3f,new List<CutsceneAction>
        {
          new InlineCutsceneAction(() => { red1.Visible=true;})
        }),
        
        new(new SpeechAction(guardNarrator,new SpeechLine("Let's Go! I don't have all day!",
            new Dictionary<string, SpeechLine>
            {
              { "Shut up the Guard and free yourself through God's power", 
                new SpeechLine(failString)
              },
              {
                "Continue",
                new SpeechLine("Yep. Knew it.")
              }
            }),4),
          new List<CutsceneAction>
          {
            new CutsceneMovementAction(jesus,new List<MovementAction>
              {
                new(j2-j1)
              },
              0,true,false,0),
            new CutsceneMovementAction(guard1,new List<MovementAction>
              {
                new(j1-initialJesusPosition)
              },
              0,true,false,0),
            new CutsceneMovementAction(guard2,new List<MovementAction>
              {
                new(j1-initialJesusPosition-guardDistance)
              },
              0,true,false,0),
          }),
        
        FlashRed(),
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(0.3f,new List<CutsceneAction>
        {
          new InlineCutsceneAction(() => { red2.Visible=true;})
        }),
        
        
        new(new SpeechAction(guardNarrator,new SpeechLine("A good service well done. Are you finished stalling?",
            new Dictionary<string, SpeechLine>
            {
              { "Shut up the Guard and free yourself through God's power", 
                new SpeechLine(failString)
              },
              {
                "Continue",
                new SpeechLine("I get my paycheck after this. Hurry it up, loser.")
              }
            }),4),
          new List<CutsceneAction>
          {
            new CutsceneMovementAction(jesus,new List<MovementAction>
              {
                new(j3-j2)
              },
              0,true,false,0),
            new CutsceneMovementAction(guard1,new List<MovementAction>
              {
                new(j2-j1)
              },
              0,true,false,0),
            new CutsceneMovementAction(guard2,new List<MovementAction>
              {
                new(j2-j1-guardDistance)
              },
              0,true,false,0),
          }),
        
        FlashRed(),
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(0.3f,new List<CutsceneAction>
        {
          new InlineCutsceneAction(() => { red3.Visible=true;})
        }),
        
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(2f, new List<CutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.ToColor(Colors.Black, 3f);
            Flash.StayStagnant(4);
            Flash.Dissolve(4);
            Flash.Initiate();
          })
        }),
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(3f, new List<CutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            crossLayer.Visible = true;
            jesus.Position = new Vector2(48*16,-24*16);
          })
        }),
        
        
        new(new SpeechAction(guardNarrator,new SpeechLine("So, after 6 hours of grueling torture; Jesus, the " +
                                                          "Son of God who did not sin, died on the cross and " +
                                                          "paid for the penalty of our sin for those who " +
                                                          "believe in him."),7),new List<CutsceneAction>()),
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(2f, new List<CutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.ToColor(Colors.Black,4);
            Flash.Initiate();
          })
        }),
        
        new(new SpeechAction(narrator,new SpeechLine("'For the wages of sin is death, but the gift of God is " +
                                                          "eternal life in Christ Jesus our Lord.' - Romans 6:23"),7),
          new List<CutsceneAction>()),
        
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(4)
        
      });
    
    return new ObjectiveDisplayGroup(new List<IHasObjective> { cutsceneObjective,negativeObjective });
  }


  private Tuple<SpeechAction, List<CutsceneAction>> FlashRed()
  {
    return SimpleCutsceneObjective.DelayedActionsWithoutSpeech(1, new List<CutsceneAction>
    {
      new InlineCutsceneAction(() =>
      {
        Flash.ToColor(Colors.Red, 0.3f);
        Flash.StayStagnant(0.4f);
        Flash.Dissolve(0.3f);
        Flash.Initiate();
      })
    });
  }
  
}