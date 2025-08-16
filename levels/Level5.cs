using System;
using System.Collections.Generic;
using Godot;
using SpiritualAdventure.cutscene.actions;
using SpiritualAdventure.entities;
using SpiritualAdventure.objectives;
using SpiritualAdventure.ui;

namespace SpiritualAdventure.levels;

public partial class Level5 : Level
{
  public override void _Ready()
  {

    var objectivesList = new List<ObjectiveDisplayGroup>
    {
      DescriptionCutsceneObjective(),JosephObjective(),CutsceneObjective()
    };

    AppendBuilder(LevelBuilder.Init()
      .SetPlayerPosition(Vector2.Zero)
      .AppendIObjectiveGroups(objectivesList));

    LoadLevel();
    
    NextObjective();
  }

  public ObjectiveDisplayGroup DescriptionCutsceneObjective()
  {
    SimpleCutsceneObjective cutsceneObjective = new(
      new List<Tuple<SpeechAction, List<ICutsceneAction>>>
      {
        new(new SpeechAction(new Narrator(),
          SimpleLinearSpeechBuilder.Of(Narrator.Identity,new List<string>
          {
            "We are set in the time before Jesus was born.",
            "The father of Jesus is Joseph - however, because his wife Mary is unexpectedly pregnant, he " +
            "wishes to divorce her quietly, as he cannot explain how she has become pregnant!",
            "As an angel sent down from heaven by God, encourage Joseph to support Mary and give birth to Jesus Christ!"
          }),1), new List<ICutsceneAction>())
      });

    return new ObjectiveDisplayGroup(new List<IHasObjective>{cutsceneObjective});
  }
  
  public ObjectiveDisplayGroup CutsceneObjective()
  {

    var initialPosition = new Vector2(1603f, 714f);
    
    
    PathDeterminantNpc mary=PathDeterminantNpc.Instantiate().UpdateMovement(new List<MovementAction>(),
      0,true,false);
    mary.Position = initialPosition;
    mary.Who(Speaker.Archer,"Mary");
    
    PathDeterminantNpc joseph = PathDeterminantNpc.Instantiate().UpdateMovement(new List<MovementAction>(),
      0,true,false);
    joseph.Position = initialPosition + new Vector2(110, 0);
    joseph.Who(Speaker.Red_Cowboy,"Joseph");

    AddChild(mary);
    AddChild(joseph);
    

    var spawnJesus = new InlineCutsceneAction(() =>
    {
      joseph.SetDirection(-1);
      Npc jesus = Npc.Instantiate().WithPosition(new Vector2(2313f,713f));
      jesus.Who(Speaker.Layman,"Jesus");
      AddChild(jesus);
    });

    SimpleCutsceneObjective cutsceneObjective=new(
      new List<Tuple<SpeechAction, List<ICutsceneAction>>> {
        
        SimpleCutsceneObjective.DelayedActionGroupWithoutSpeech(0,new List<ICutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.ToColor(new Color(1,1,1,0),0);
            Flash.ToSolid(0.5f);
            Flash.Initiate();
          })
        }),
        
        SimpleCutsceneObjective.DelayedActionGroupWithoutSpeech(1.5f,new List<ICutsceneAction>
        {
          new PanCutsceneAction(new Vector2(2303f, 530f)),
          new InlineCutsceneAction(() =>
          {
            Flash.Dissolve(2);
            Flash.Initiate();
          })
        }),
        
        new(new SpeechAction(narrator,new SpeechLine(Narrator.Identity,"And so, Joseph and Mary traveled safely to Bethlehem in God's " +
                                                                      "protection, stopping to rest at an old stable..." ),1),
          new List<ICutsceneAction>
          {
            new MovementCutsceneAction(mary,new List<MovementAction>{new (657f,0)},
              0,true,false,0),
            new MovementCutsceneAction(joseph,new List<MovementAction>{new (657f,0)},
              0,true,false,0) 
          }),
        
        new(new SpeechAction(narrator,new SpeechLine(Narrator.Identity,"And baby Jesus was born. "),7),
          new List<ICutsceneAction> {spawnJesus}),
        
        SimpleCutsceneObjective.DelayedActionGroupWithoutSpeech(1)
        
      });
    
    return new ObjectiveDisplayGroup(new List<IHasObjective>{cutsceneObjective});
  }
  
  public ObjectiveDisplayGroup JosephObjective()
  {
    Identity josephIdentity = new(Speaker.Red_Cowboy,"Joseph");
    
    
    var npc=Npc.Instantiate().WithPosition(new Vector2(872f,712f));
    AddChild(npc);

    const string verse = "Joseph, son of David, do not be afraid to take Mary your wife into your home. " +
                         "For it is through the Holy Spirit that this child has been conceived in her. " +
                         "She will bear a son and you are to name him Jesus, " +
                         "because he will save his people from their sins.";

    const string targetLine = "What!? I must follow the Lord's request, regardless of my moral intuitions!";
    
    
    var speechLines = new List<SpeechLine>
    { 
      new SpeechLine(josephIdentity,"Snore....      ..?   <What should you say in his dream?>")
        .SetOptions(
          new Dictionary<string, SpeechLine>
          {
            {"WRONG OPTION 1",new SpeechLine(Narrator.Identity,"This should FAIL the level")},
            {verse,new SpeechLine(josephIdentity,targetLine)},
            {"WRONG OPTION 2",new SpeechLine(Narrator.Identity,"This should FAIL the level")},
            {"WRONG OPTION 3",new SpeechLine(Narrator.Identity,"This should FAIL the level")}
          }
        )
    };
    
    npc.Who(josephIdentity);
    npc.UseTrigger("interact","Talk");
    npc.SetSpeech(speechLines);
    
    var rawChatObjective = new Objective("Descend from Heaven and inform Joseph about giving birth to Jesus");
    StartChatObjective startChatObjective=new(npc,rawChatObjective);

    var rawOptionObjective = new Objective("Deliver the correct message in Joseph's dream");
    OptionObjective optionObjective = new(verse,rawOptionObjective,
      new[]{"WRONG OPTION 1","WRONG OPTION 2","WRONG OPTION 3"});

    var rawTargetSpeechObjective = new Objective("Finish talking to Joseph");
    TargetSpeechObjective targetSpeechObjective = new(targetLine,rawTargetSpeechObjective);


    return new ObjectiveDisplayGroup(new List<IHasObjective>
    {
      startChatObjective, optionObjective, targetSpeechObjective
    });
  }
  
  
}