using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

public partial class Level5 : Level
{
  public override void _Ready()
  {

    var objectivesList = new List<ObjectiveDisplayGroup>
    {
      GospelCutsceneObjective(),DescriptionCutsceneObjective(),JosephObjective(),CutsceneObjective()
    };
    
    LoadLevel(new Vector2(0,0),objectivesList,new List<Npc>(),new Narrator());
    NextObjective();
  }

  public ObjectiveDisplayGroup GospelCutsceneObjective()
  {

    string gospelVerse = "Romans 6:23 - 'For the wages of sin is death, " +
                         "but the gift of God is eternal life in Christ Jesus our Lord.";
    
    var prologue = SimpleLinearSpeechBuilder.Of(new List<string>
    {
      "In the next 3 levels, we will be exploring the gospel. The gospel is a term to describe 'good news'.",
      "Before we describe what the gospel is in accordance with Christianity, we must first define a few terms.",
      "The verse we will be unpacking to describe the gospel is "+gospelVerse+
      " This verse is provided at the top left of the screen as reference.",
    });

    var wageQuestion = new SpeechLine();
    var wageCorrect = new SpeechLine();
    var wageIncorrect = new SpeechLine();
    var wageExampleAfterQuestion = new SpeechLine();

    
    var sinQuestion = new SpeechLine();
    var sinCorrect = new SpeechLine();
    var sinIncorrect = new SpeechLine();
    var sinExampleAfterQuestion = new SpeechLine();
    
    var understandingWagesAndSin = new SpeechLine();
    
    var giftQuestion = new SpeechLine();
    var giftCorrect = new SpeechLine();
    var giftIncorrect = new SpeechLine();
    var giftExampleAfterQuestion = new SpeechLine();
    
    var eternalLifeQuestion = new SpeechLine();
    var eternalLifeCorrect = new SpeechLine();
    var eternalLifeIncorrect = new SpeechLine();
    
    var understandingGodLove = new SpeechLine();
    
    prologue.LastLine().next = wageQuestion;
    wageQuestion.Update("Let's unpack what this is saying. What do you think the word 'wage' means in this context?",
      new Dictionary<string, SpeechLine>
      {
        { "A wage is a reward you get in return for some sort of work.", wageCorrect },
        {"INCORRECT ANSWER",wageIncorrect}
      });
    wageCorrect.Update("Nice job! You're completely right.",wageExampleAfterQuestion);
    wageIncorrect.Update("Close. The correct answer was " +
                         "'A wage is a reward you get in return for some sort of work.'",wageExampleAfterQuestion);
    wageExampleAfterQuestion.Update("An easy example of a wage can be seen through jobs. " +
                                    "People work in jobs to earn wages, usually money.",sinQuestion);
    
    
    sinQuestion.Update("Okay, easy enough. Now what do you think is sin? This one is a tough question if " +
                       "you've never been exposed to Christianity before.",
      new Dictionary<string, SpeechLine>
      {
        {"'Missing the mark'. Something must be perfect in order to be regarded as sinless.",sinCorrect},
        {"INCORRECT ANSWER",sinIncorrect}
      });
    sinCorrect.Update("Great work! You definitely know your stuff.",sinExampleAfterQuestion);
    sinIncorrect.Update("Almost. The correct answer was 'Missing the mark'. " +
                        "Something must be perfect in order to be regarded as sinless.",sinExampleAfterQuestion);
    
    sinExampleAfterQuestion.Update("Ever lied to your parents? Or gotten angry at someone? " +
                                   "Or judged someone at all? These are all examples of sin.",understandingWagesAndSin);
    
    understandingWagesAndSin.Update("Now that we have those definitions cleared up, we can derive the meaning" +
                                    "of the first half, or 'The wages of sin is death'. ",
      SimpleLinearSpeechBuilder.Of(new List<string>
      {
        "A wage is receiving something in return for your actions. We receive death for committing sin.",
        "The rough part about this is that every human is not perfect. So what this is saying is that every single " +
        "human on Earth is subject to death through sin.",
        "Pretty gloomy right? Well, don't be afraid. There is a way out."
      }));
    understandingWagesAndSin.LastLine().next = giftQuestion;
    
    giftQuestion.Update("What do you think 'gift' means?",
      new Dictionary<string, SpeechLine>
      {
        {"A gift is a undeserved reward.",giftCorrect},
        {"INCORRECT ANSWER",giftIncorrect}
      });
    giftCorrect.Update("Nice one! You're absolutely correct.",giftExampleAfterQuestion);
    giftIncorrect.Update("Not quite. The answer was 'A gift is a undeserved reward.'",giftExampleAfterQuestion);
    giftExampleAfterQuestion.Update("One example of a gift would receiving a Thomas the Train toy during Christmas.",
      new SpeechLine("A gift is free and undeserving: you never had to lift a finger for that shiny blue engine.",
        eternalLifeQuestion));
    
    eternalLifeQuestion.Update("What do you think 'eternal life' means in this context?",
      new Dictionary<string, SpeechLine>
      {
        {"Eternal life is the ultimate reward. It is an infinite life of bliss " +
         "spurred by a personal relationship with God.",eternalLifeCorrect},
        {"INCORRECT ANSWER",eternalLifeIncorrect}
      });
    eternalLifeCorrect.Update("Yep! It's definitely the place you want to be.",understandingGodLove);
    eternalLifeIncorrect.Update("Not exactly. The correct answer was 'Eternal life is the ultimate reward. " +
                                "It is an infinite life of bliss spurred by a personal relationship with God.'",
      understandingGodLove);
    
    understandingGodLove.Update("Now, even though we deserved death through our sin, God provides a gift " +
                                "of eternal life through Jesus.",
      SimpleLinearSpeechBuilder.Of(new List<string>
      {
        "Who is Jesus, you may ask? Well, he goes by many names. The Messiah, the Savior, Son of God, Son of Man, " +
        "and many others. But the main thing you need to know about him right now is that he paid for the penalty " +
        "of everyone's sins.",
        "We will get into how Jesus is the bridge from death to eternal life, but first I want you to " +
        "observe the life of Jesus through the next 3 levels. Have fun!"
      }));
    
    return new ObjectiveDisplayGroup(new List<IHasObjective>{
      new SimpleCutsceneObjective(new List<Tuple<SpeechAction, List<CutsceneAction>>>
      {
        new (new SpeechAction(new Narrator(Speaker.Layman,"Narrator"),prologue,1),
          new List<CutsceneAction>()),
      }),
      new NegativeObjective(new Objective("Understand "+gospelVerse),new List<Objective>())
    });
  }

  public ObjectiveDisplayGroup DescriptionCutsceneObjective()
  {
    SimpleCutsceneObjective cutsceneObjective = new(
      new List<Tuple<SpeechAction, List<CutsceneAction>>>
      {
        new(new SpeechAction(new Narrator(Speaker.Layman,"Narrator"),
          SimpleLinearSpeechBuilder.Of(new List<string>
          {
            "We are set in the time before Jesus was born.",
            "The father of Jesus is Joseph - however, because his wife Mary is unexpectedly pregnant, he " +
            "wishes to divorce her quietly, as he cannot explain how she has become pregnant!",
            "As an angel sent down from heaven by God, encourage Joseph to support Mary and give birth to Jesus Christ!"
          }),1), new List<CutsceneAction>())
      });

    return new ObjectiveDisplayGroup(new List<IHasObjective>{cutsceneObjective});
  }
  
  public ObjectiveDisplayGroup CutsceneObjective()
  {

    var initialPosition = new Vector2(1603f, 714f);
    
    
    PathDeterminantNpc mary=PathDeterminantNpc.Instantiate(new List<MovementAction>(),initialPosition,
      0,true,false);
    mary.Who(Speaker.Archer,"Mary");
    
    PathDeterminantNpc joseph = PathDeterminantNpc.Instantiate(new List<MovementAction>(),
      initialPosition+new Vector2(110,0),0,true,false);
    joseph.Who(Speaker.Red_Cowboy,"Joseph");

    AddChild(mary);
    AddChild(joseph);
    

    var spawnJesus = new InlineCutsceneAction(() =>
    {
      joseph.SetDirection(-1);
      Npc jesus = Npc.Instantiate(new Vector2(2313f,713f));
      jesus.Who(Speaker.Layman,"Jesus");
      AddChild(jesus);
    });
    

    var narrator = new Narrator(Speaker.Archer,"Narrator");
    SimpleCutsceneObjective cutsceneObjective=new(
      new List<Tuple<SpeechAction, List<CutsceneAction>>> {
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(0,new List<CutsceneAction>
        {
          new InlineCutsceneAction(() =>
          {
            Flash.ToColor(new Color(1,1,1,0),0);
            Flash.ToSolid(0.5f);
            Flash.Initiate();
          })
        }),
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(1.5f,new List<CutsceneAction>
        {
          new PanCutsceneAction(new Vector2(2303f, 530f)),
          new InlineCutsceneAction(() =>
          {
            Flash.Dissolve(2);
            Flash.Initiate();
          })
        }),
        
        new(new SpeechAction(narrator,new SpeechLine("And so, Joseph and Mary traveled safely to Bethlehem in God's " +
                                                     "protection, stopping to rest at an old stable..." ),1),
          new List<CutsceneAction>
          {
            new CutsceneMovementAction(mary,new List<MovementAction>{new (657f,0)},
              0,true,false,0),
            new CutsceneMovementAction(joseph,new List<MovementAction>{new (657f,0)},
              0,true,false,0) 
          }),
        
        new(new SpeechAction(narrator,new SpeechLine("And baby Jesus was born. "),7),
          new List<CutsceneAction> {spawnJesus}),
        
        SimpleCutsceneObjective.DelayedActionsWithoutSpeech(1)
        
      });
    
    return new ObjectiveDisplayGroup(new List<IHasObjective>{cutsceneObjective});
  }
  
  public ObjectiveDisplayGroup JosephObjective()
  {
    var npc=Npc.Instantiate(new Vector2(872f,712f));
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
    
    npc.Who(Speaker.Red_Cowboy,"Joseph");
    npc.UseTrigger("interact","Talk");
    npc.SetSpeech(speechLines);
    
    var rawChatObjective = new Objective("Descend from Heaven and inform Joseph about giving birth to Jesus");
    StartChatObjective startChatObjective=new(npc,rawChatObjective);

    var rawOptionObjective = new Objective("Deliver the correct message in Joseph's dream");
    OptionObjective optionObjective = new(verse,rawOptionObjective,
      new[]{"WRONG OPTION 1","WRONG OPTION 2","WRONG OPTION 3"});

    var rawTargetSpeechObjective = new Objective("Finish talking to Joseph");
    TargetSpeechObjective targetSpeechObjective = new(rawTargetSpeechObjective,targetLine);


    return new ObjectiveDisplayGroup(new List<IHasObjective>
    {
      startChatObjective, optionObjective, targetSpeechObjective
    });
  }
  
  
}