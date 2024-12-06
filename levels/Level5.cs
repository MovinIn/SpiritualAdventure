using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

public partial class Level5 : Level
{
  public override void _Ready()
  {
    
    var combinedObjectivesList = JosephObjectives().Concat(CutsceneObjective()).ToList();
    
    LoadLevel(new Vector2(0,0),combinedObjectivesList,new List<Npc>(),new Narrator());
    NextObjective();
  }

  public List<IHasObjective> CutsceneObjective()
  {

    var initialPosition = new Vector2(1603.8605f, 714.938f);
    
    
    PathDeterminantNpc mary=PathDeterminantNpc.Instantiate(new List<MovementAction>(),initialPosition,
      0,true,false);
    mary.Who(Speaker.Archer,"Mary");
    
    PathDeterminantNpc joseph = PathDeterminantNpc.Instantiate(new List<MovementAction>(),
      initialPosition+new Vector2(110,0),0,true,false);
    joseph.Who(Speaker.Red_Warrior,"Joseph");

    AddChild(mary);
    AddChild(joseph);
    
    // (1603.8605, 714.938)
    
    // (2345.0134, 713.32605)
    // (2261.3262, 713.32605)
    // (2313.6738, 713.32605)

    var spawnJesus = new InlineCutsceneAction(() =>
    {
      Npc jesus = Npc.Instantiate(new Vector2(2313.6738f,713.32605f));
      jesus.Who(Speaker.Layman,"Jesus");
      AddChild(jesus);
    });
    

    var narrator = new Narrator(Speaker.Archer,"Narrator");
    SimpleCutsceneObjective cutsceneObjective=new(
      new List<Tuple<SpeechAction, List<CutsceneAction>>> {
        new(new SpeechAction(narrator,new SpeechLine("And so, Joseph and Mary traveled safely to Bethlehem in God's " +
                                                     "protection, stopping to rest at an old stable..." ),1),
          new List<CutsceneAction>
          {
            new CutsceneMovementAction(mary,new List<MovementAction>{new (657.4657f,0)},
              0,true,false,0),
            new CutsceneMovementAction(joseph,new List<MovementAction>{new (657.4657f,0)},
            0,true,false,0)
          }),
        new(new SpeechAction(narrator,new SpeechLine("And baby Jesus was born. "),5),
          new List<CutsceneAction> {spawnJesus}),
        new(new SpeechAction(narrator,null,1),
          new List<CutsceneAction>())
      }, new Vector2(2303.1116f, 530.73663f));
    return new List<IHasObjective> {cutsceneObjective};
  }
  
  public List<IHasObjective> JosephObjectives()
  {
    var npc=Npc.Instantiate(new Vector2(872.67f,712.67f));
    AddChild(npc);

    const string verse = "Joseph, son of David, do not be afraid to take Mary your wife into your home. " +
                         "For it is through the Holy Spirit that this child has been conceived in her. " +
                         "She will bear a son and you are to name him Jesus, " +
                         "because he will save his people from their sins.";

    const string targetLine = "What!? I must follow the Lord's request, regardless of my moral intuitions!";
    
    
    var speechLines = new List<SpeechLine>
    { 
      new("Snore....      ..?   <What should you say in his dream?>",
        new Dictionary<string, SpeechLine>
        {
          {"WRONG OPTION 1",new SpeechLine("This should FAIL the level")},
          {verse,new SpeechLine(targetLine)},
          {"WRONG OPTION 2",new SpeechLine("This should FAIL the level")},
          {"WRONG OPTION 3",new SpeechLine("This should FAIL the level")}
        })
    };
    
    npc.Who(Speaker.Red_Warrior,"Joseph");
    npc.UseTrigger("interact","Talk");
    npc.SetSpeech(speechLines);
    
    var rawChatObjective = new Objective("Descend from Heaven and inform Joseph about giving birth to Jesus");
    StartChatObjective startChatObjective=new(npc,rawChatObjective);

    var rawOptionObjective = new Objective("Deliver the correct message in Joseph's dream");
    OptionObjective optionObjective = new(npc,verse,rawOptionObjective,
      new[]{"WRONG OPTION 1","WRONG OPTION 2","WRONG OPTION 3"});

    var rawTargetSpeechObjective = new Objective("Finish talking to Joseph");
    TargetSpeechObjective targetSpeechObjective = new(npc,rawTargetSpeechObjective,targetLine);
    
    
    return new List<IHasObjective> {startChatObjective,optionObjective,targetSpeechObjective};
  }
  
  
}