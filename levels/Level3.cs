using Godot;
using System.Collections.Generic;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

public partial class Level3 : LevelWithTestExtensions
{

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    TestMultipleObjectives();
  }

  private void LoadWithObjectives(List<ObjectiveDisplayGroup>objectives)
  {
    Npc n=TestNpc(new Vector2(200,200));

    var builder = LevelBuilder.Init()
      .SetPlayerPosition(Vector2.Zero)
      .AppendIObjectiveGroups(objectives)
      .AppendNpcList(new List<Npc> { n })
      .SetNarrator(new Narrator());
    
    AppendBuilder(builder);

    LoadLevel();
    
  }

  public void TestMultipleObjectives()
  {
    List<ObjectiveDisplayGroup> objectives=new()
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

  public Npc TestNpc(Vector2 position)
  {
    Npc npc=PathDeterminantNpc.Instantiate().UpdateMovement(new List<MovementAction>{new (100,0),
        new (100,100,1.5d),new (0,100),new (0,0)},2,true,true);
    npc.Position = position;
    npc.Who(new Identity(Speaker.Red_Warrior,"Roman"));
    npc.UseTrigger("interact","Talk");
    var speechLines = new List<SpeechLine>
    {
      SimpleLinearSpeechBuilder.Of(npc.identity,new List<string>
      {
        "Hello! My name is Roman and I love soccer.", "Test Text 2", "Test Text 3"
      }),
      new SpeechLine(npc.identity,
        "Here is a list of options you can choose from. Option 3 has more extensive dialogue.")
        .SetOptions(new Dictionary<string, SpeechLine>
        {
          { "Option 1", new SpeechLine(npc.identity,"You chose option 1! Nice :)") },
          { "Option 2", new SpeechLine(npc.identity,"You chose option 2! Nice :)") },
          {
            "Option 3", new SpeechLine(npc.identity,"You chose option 3! Nice :)")
              .SetNext(SimpleLinearSpeechBuilder.Of(npc.identity,new List<string> {
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
