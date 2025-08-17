using Godot;
using System;
using System.Collections.Generic;
using SpiritualAdventure.cutscene.actions;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;

public partial class Level4 : Level
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    AppendBuilder(LevelBuilder.Init()
      .SetPlayerPosition(Vector2.Zero)
      .AppendIObjectiveGroups(new List<ObjectiveDisplayGroup> { GospelCutsceneObjective() }));
    
    LoadLevel();
    
    NextObjective();
  }
  
    public ObjectiveDisplayGroup GospelCutsceneObjective()
  {

    string gospelVerse = "Romans 6:23 - 'For the wages of sin is death, " +
                         "but the gift of God is eternal life in Christ Jesus our Lord.'";

    var prologue = SimpleLinearSpeechBuilder.Of(Narrator.Identity,new List<string>
    {
      "In the next 3 levels, we will be exploring the gospel. The gospel is a term to describe 'good news'.",
      "Before we describe what the gospel is in accordance with Christianity, we must first define a few terms.",
      "The verse we will be unpacking to describe the gospel is "+gospelVerse+
      " This verse is provided at the top left of the screen as reference."
    });

    var wageQuestion = new SpeechLine(Narrator.Identity,"Let's unpack what this is saying. What do you think the word 'wage' means in this context?");
    var wageCorrect = new SpeechLine(Narrator.Identity,"Nice job! You're completely right.");
    var wageIncorrect = new SpeechLine(Narrator.Identity,"Close. The correct answer was 'A wage is a reward you get in return for some sort of work.'");
    var wageExampleAfterQuestion = new SpeechLine(Narrator.Identity,"An easy example of a wage can be seen through jobs. People work in jobs to earn wages, usually money.");

    
    var sinQuestion = new SpeechLine(Narrator.Identity,"Okay, easy enough. Now, what do you think is sin? This one is a tough question if you’ve never been exposed to Christianity before.");
    var sinCorrect = new SpeechLine(Narrator.Identity,"Great work! You definitely know your stuff.");
    var sinIncorrect = new SpeechLine(Narrator.Identity,"Almost. The correct answer was “missing the mark.” Something must be perfect in order to be regarded as sinless.");
    var sinExampleAfterQuestion = new SpeechLine(Narrator.Identity,"Ever lied to your parents? Or gotten angry at someone? Or judged someone at all? These are all examples of sin.");
    
    var understandingWagesAndSin = new SpeechLine(Narrator.Identity,"Now that we have those definitions cleared up, we can derive the meaning of the first half, or “The wages of sin is death.”");
    
    var giftQuestion = new SpeechLine(Narrator.Identity,"What do you think 'gift' means?");
    var giftCorrect = new SpeechLine(Narrator.Identity,"Nice one! You're absolutely correct.");
    var giftIncorrect = new SpeechLine(Narrator.Identity,"Not quite. The answer was 'A gift is a undeserved reward.'");
    var giftExampleAfterQuestion = new SpeechLine(Narrator.Identity,"One example of a gift would receiving a Thomas the Train toy during Christmas.");
    
    var eternalLifeQuestion = new SpeechLine(Narrator.Identity,"What do you think 'eternal life' means in this context?");
    var eternalLifeCorrect = new SpeechLine(Narrator.Identity,"Yep! It's definitely the place you want to be.");
    var eternalLifeIncorrect = new SpeechLine(Narrator.Identity,"Not exactly. The correct answer was “Eternal life is the ultimate reward. It is an infinite life of bliss spurred by a personal relationship with God.”");
    
    var understandingGodLove = new SpeechLine(Narrator.Identity,"Now, even though we deserved death through our sin, God provides a gift of eternal life through Jesus.");
    
    prologue.LastLine().next = wageQuestion;
    wageQuestion.SetOptions(
      new Dictionary<string, SpeechLine>
      {
        { "A wage is a reward you get in return for some sort of work.", wageCorrect },
        {"INCORRECT ANSWER",wageIncorrect}
      });
    wageCorrect.SetNext(wageExampleAfterQuestion);
    wageIncorrect.SetNext(wageExampleAfterQuestion);
    wageExampleAfterQuestion.SetNext(sinQuestion);
    
    sinQuestion.SetOptions(
      new Dictionary<string, SpeechLine>
      {
        {"'Missing the mark'. Something must be perfect in order to be regarded as sinless.",sinCorrect},
        {"INCORRECT ANSWER",sinIncorrect}
      });
    sinCorrect.SetNext(sinExampleAfterQuestion);
    sinIncorrect.SetNext(sinExampleAfterQuestion);
    
    sinExampleAfterQuestion.SetNext(understandingWagesAndSin);
    
    understandingWagesAndSin.SetNext(
      SimpleLinearSpeechBuilder.Of(Narrator.Identity,new List<string>
      {
        "A wage is receiving something in return for your actions. We receive death for committing sin.",
        "The rough part about this is that every human is not perfect. So what this is saying is that every single " +
        "human on Earth is subject to death through sin.",
        "Pretty gloomy right? Well, don't be afraid. There is a way out."
      }));
    understandingWagesAndSin.LastLine().next = giftQuestion;
    
    giftQuestion.SetOptions(
      new Dictionary<string, SpeechLine>
      {
        {"A gift is a undeserved reward.",giftCorrect},
        {"INCORRECT ANSWER",giftIncorrect}
      });
    giftCorrect.SetNext(giftExampleAfterQuestion);
    giftIncorrect.SetNext(giftExampleAfterQuestion);
    giftExampleAfterQuestion.SetNext(
      new SpeechLine(Narrator.Identity,"A gift is free and undeserving: you never had to lift a " +
                                      "finger for that shiny blue engine."));
    giftExampleAfterQuestion.LastLine().next = eternalLifeQuestion;
    
    eternalLifeQuestion.SetOptions(
      new Dictionary<string, SpeechLine>
      {
        {"Eternal life is the ultimate reward. It is an infinite life of bliss " +
         "spurred by a personal relationship with God.",eternalLifeCorrect},
        {"INCORRECT ANSWER",eternalLifeIncorrect}
      });
    eternalLifeCorrect.SetNext(understandingGodLove);
    eternalLifeIncorrect.SetNext(understandingGodLove);
    
    understandingGodLove.SetNext(
      SimpleLinearSpeechBuilder.Of(Narrator.Identity,new List<string>
      {
        "Who is Jesus, you may ask? Well, he goes by many names. The Messiah, the Savior, Son of God, Son of Man, " +
        "and many others. But the main thing you need to know about him right now is that he paid for the penalty " +
        "of everyone's sins.",
        "We will get into how Jesus is the bridge from death to eternal life, but first I want you to " +
        "observe the life of Jesus through the next 3 levels. Have fun!"
      }));
    
    return ObjectiveDisplayGroup.Builder.Init(new List<IHasObjective>{
      new SimpleCutsceneObjective(new List<Tuple<SpeechAction, List<ICutsceneAction>>>
      {
        new (new SpeechAction(new Narrator(),prologue,1),
          new List<ICutsceneAction>()),
      }),
      new NegativeObjective(new Objective("Understand "+gospelVerse),new List<Objective>())
    }).Build();
  }
}
