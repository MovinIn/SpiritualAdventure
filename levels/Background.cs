using System.Collections.Generic;
using Godot;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

public partial class Background : Level
{
  Player player;
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    player=GetNode<Player>("Player");
    testNPC(new Vector2(200,200));
    testObjective(new Vector2(100,100));
  }

  public void testObjective(Vector2 position)
  {
    TouchObjective o=TouchObjective.Instantiate("Touch The Checkpoint");
    o.Position = position;
    Queue<Objective> a = new Queue<Objective>();
    a.Enqueue(o.objective);
    LoadLevel(a,new List<Npc>());
    AddChild(o);
    NextObjective();
  }
  
  public void testNPC(Vector2 position)
  {
    Npc npc=PathDeterminantNpc.Instantiate(
      new List<Action>{new (100,0),new (100,100,1.5d),new (0,100),new (0,0)},
      position,2,true);
    AddChild(npc);
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
    string json = JsonSpeechDeserializer.Serialize(speechLines);
    GD.Print(json);
    speechLines=JsonSpeechDeserializer.Deserialize(json);
    npc.SetSpeech(speechLines);
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
  }
}