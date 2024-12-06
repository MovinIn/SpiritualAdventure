using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

public partial class Level3 : Level
{

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    TestMultipleObjectives();
  }

  private void LoadWithObjectives(List<IHasObjective>objectives)
  {
    Npc n=TestNpc(new Vector2(200,200));
    LoadLevel(new Vector2(0,0),objectives,new List<Npc>{n},new Narrator());
  }

  public void TestMultipleObjectives()
  {
    List<IHasObjective> objectives=new()
    {
      TestChatObjective(new Vector2(150, 150)),
      TestTouchObjective(new Vector2(200, 0)),
      TestChatObjective(new Vector2(300,0),10),
      TestTouchObjective(new Vector2(500,0),20),
      TestOptionObjective(new Vector2(700,-100),30)
    };
    LoadWithObjectives(objectives);
    NextObjective();
  }
  
  public IHasObjective TestChatObjective(Vector2 position,int timeLimit=-1)
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
    var objective = ObjectiveBuilder.TimedOrElse("Talk to the NPC at (150,150)", timeLimit);
    objective.postCompletionFeedback = new SpeechLine("This is a MULTI-LINE Post Completion Feedback Line!",
      new SpeechLine("A special treatment for NPCs!"));
    StartChatObjective c=new(npc,objective);
    return c;
  }

  public IHasObjective TestOptionObjective(Vector2 position,int timeLimit=-1)
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
    var optionObjective = new OptionObjective(npc, "Option 2",ObjectiveBuilder.TimedOrElse(
      "Choose the correct objective", timeLimit),new[]{"Option 3"});
    return optionObjective;
  }
  
  public IHasObjective TestTouchObjective(Vector2 position,int timeLimit=-1)
  {
    var touchObjective=TouchObjective.Instantiate(ObjectiveBuilder.TimedOrElse(
      "Touch the Checkpoint",timeLimit));
    touchObjective.Position = position;
    touchObjective.objective.postCompletionFeedback = new SpeechLine("This is a Post Completion Feedback Line FOR TOUCH!");
    AddChild(touchObjective);
    return touchObjective;
  }
  
  public Npc TestNpc(Vector2 position)
  {
    Npc npc=PathDeterminantNpc.Instantiate(
      new List<MovementAction>{new (100,0),new (100,100,1.5d),new (0,100),new (0,0)},
      position,2,true,true);
    npc.Who(Speaker.Red_Warrior,"Roman");
    npc.UseTrigger("interact","Talk");
    var speechLines = new List<SpeechLine>
    {
      SimpleLinearSpeechBuilder.of(new List<string>
      {
        "Hello! My name is Roman and I love soccer.", "Test Text 2", "Test Text 3"
      }),
      new("Here is a list of options you can choose from. Option 3 has more extensive dialogue.",
        new Dictionary<string, SpeechLine>
        {
          { "Option 1", new SpeechLine("You chose option 1! Nice :)") },
          { "Option 2", new SpeechLine("You chose option 2! Nice :)") },
          {
            "Option 3", new SpeechLine("You chose option 3! Nice :)",
              SimpleLinearSpeechBuilder.of(new List<string>
              {
                "This speech is really really hard to write, " +
                "so we should make a speech parser that parses" +
                " a text file one day :D"
              }))
          }
        })
    };
    
    string speechLinesJson = JsonSpeechDeserializer.Serialize(speechLines);
    speechLines=JsonSpeechDeserializer.Deserialize(speechLinesJson);
    npc.SetSpeech(speechLines);
    return npc;
  }
}
