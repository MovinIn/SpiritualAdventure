using Godot;
using System;
using System.Collections.Generic;
using SpiritualAdventure.cutscene.actions;
using SpiritualAdventure.entities;
using SpiritualAdventure.levels;
using SpiritualAdventure.objectives;
using SpiritualAdventure.objects;
using SpiritualAdventure.ui;

public partial class Level6 : Level
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {

    var builder = LevelBuilder.Init()
      .SetPlayerPosition(Vector2.Zero)
      .AppendIObjectiveGroups(new List<ObjectiveDisplayGroup>
      {
        BlindCutsceneObjective(), TouchHomeObjective(), JesusCutsceneObjective()
      })
      .AppendNpcList(new List<Npc>())
      .SetNarrator(new Narrator());
    
    AppendBuilder(builder);
    
    LoadLevel();
    
    NextObjective();
  }

  
  public ObjectiveDisplayGroup BlindCutsceneObjective()
  {
    var jesusIdentity = new Identity(Speaker.Layman, "Jesus");
    var youIdentity = new Identity(Speaker.Layman,"You");

    string targetOption = "Wash in the Pool of Siloam";
    var optionObjective = new OptionObjective(targetOption, new Objective("Obey Jesus"),
      new []{"Do not"});
    
	
    SimpleCutsceneObjective cutsceneObjective = new(new List<Tuple<SpeechAction, List<ICutsceneAction>>>
    {
	  
      SimpleCutsceneObjective.DelayedActionGroupWithoutSpeech(0, new List<ICutsceneAction>
      {
        new InlineCutsceneAction(() =>
        {
          Flash.ToColor(new Color(0,0,0), 0);
          Flash.Initiate();
        })
      }),
	  
      new(new SpeechAction(narrator,
          new SpeechLine(Narrator.Identity,
              "The scene we are now depicting is Jesus healing a blind man. " +
              "Through this, he reveals he is the Son of Man " +
              "(and in later levels we will show scripture detailing him as " +
              "the one and only Son of God), and also brings to light the evil " +
              "in the Pharisees and Jewish leaders (who would eventually crucify " +
              "Him).")
            .SetNext(new SpeechLine(Narrator.Identity,"If you couldn't tell already," +
                                                      " you're the blind man - you've been blind your whole life.")),2),
        new List<ICutsceneAction>()),
	  
      new(new SpeechAction(narrator,new SpeechLine(jesusIdentity,"<Spits on the ground and makes mud>")
          .SetNext(new SpeechLine(jesusIdentity,"<Puts it on the blind man's (you) eyes>")
            .SetNext(new SpeechLine(jesusIdentity,"Go, wash in the Pool of Siloam"))
            .SetOptions(new Dictionary<string, SpeechLine> {
              { targetOption, new SpeechLine(jesusIdentity,"Swish... Swish.....") },
              {"Do not", null}
            })),3),
        new List<ICutsceneAction>()),
	  
      SimpleCutsceneObjective.DelayedActionGroupWithoutSpeech(2,new List<ICutsceneAction>
      {
        new InlineCutsceneAction(() =>
        {
          Flash.ToColor(new Color(1,1,1),0.3f);
          Flash.StayStagnant(2);
          Flash.Dissolve(4);
          Flash.Initiate();
        })
      }),
	  
      new(new SpeechAction(narrator,new SpeechLine(youIdentity,"Wha..... What....? I can see again..!!")
          .SetNext(new SpeechLine(youIdentity,"I must spread the word to my neighbors: how God has blessed my life!")),8),
        new List<ICutsceneAction>())
    });
    return new ObjectiveDisplayGroup(new List<IHasObjective>{cutsceneObjective,optionObjective});
  }

  public ObjectiveDisplayGroup TouchHomeObjective()
  {
    Npc neighbor1, neighbor2, neighbor3;
    Identity i1, i2, i3;
    i1 = new Identity(Speaker.Archer,"Alivia");
    i2 = new Identity(Speaker.Black_Cowboy,"Eli");
    i3 = new Identity(Speaker.Red_Warrior,"Caleb");
    neighbor1 = Npc.Instantiate().WithPosition(new Vector2(1796,351));
    neighbor2 = Npc.Instantiate().WithPosition(new Vector2(1344,371));
    neighbor3 = Npc.Instantiate().WithPosition(new Vector2(287,446));
    AddChild(neighbor1);
    AddChild(neighbor2);
    AddChild(neighbor3);
    
    neighbor1.UseTrigger("interact","Talk");
    neighbor2.UseTrigger("interact","Talk");
    neighbor3.UseTrigger("interact","Talk");
    
    neighbor1.Who(i1);
    neighbor2.Who(i2);
    neighbor3.Who(i3);
    
    neighbor1.SetSpeech(new List<SpeechLine> {new SpeechLine(i1,"What??! Is that really you, Ryan? YOU CAN SEE!!")
      .SetNext(new SpeechLine(i1,"Trust in the Lord with all your heart, and" +
                                 " lean not on your own understanding. In all your" +
                                 " ways submit to him and he will make your paths " +
                                 "straight :) Proverbs 3:5-6"))
    });
    neighbor2.SetSpeech(new List<SpeechLine> {new SpeechLine(i2,"In...INCREDIBLE!!!! PRAISE THE LORD! ")
      .SetNext(new SpeechLine(i2,"Heal me, Lord, and I will be healed; save me and " +
                                 "I will be saved, for you are the one I praise :) " +
                                 "Jeremiah 17:14"))});
    neighbor3.SetSpeech(new List<SpeechLine>{new SpeechLine(i3,"My goodness..! I'm going to cry... ")
      .SetNext(new SpeechLine(i3,"Praise the Lord, my soul, and " +
                                 "forget not all his benefits - who forgives us of our sins" +
                                 " and heals us of our diseases Psalms 103:3-5 :) "))});
    
    
    
    var objective1=new Objective("Talk To Alivia"); //TODO: make post completion feedback for objectivedisplaygroup as well.
    var objective2=new Objective("Talk To Eli");
    var objective3=new Objective("Talk To Caleb");

    var chatObjective1 = new FinishChatObjective(neighbor1,objective1);
    var chatObjective2 = new FinishChatObjective(neighbor2, objective2);
    var chatObjective3 = new FinishChatObjective(neighbor3, objective3);
    
    return new ObjectiveDisplayGroup(new List<IHasObjective>{chatObjective1,chatObjective2,chatObjective3});
  }

  public ObjectiveDisplayGroup JesusCutsceneObjective()
  {
    var phariseeIdentity = new Identity(Speaker.Red_Merchant,"That One Pharisee");
    var romanSoldierIdentity=new Identity(Speaker.Red_Warrior,"Roman Soldiers");
    var nicodemusIdentity = new Identity(Speaker.Red_Merchant,"Nicodemus");
    var secondPhariseeIdentity = new Identity(Speaker.Red_Merchant,"That Second Pharisee");

    var jesusPosition=new Vector2(-1547f, -278f);
    var rightPhariseePosition = new Vector2(-1218f, 474f);
    var phariseeCutscenePosition = new Vector2(-1488f, 473f);

    Npc[] npcArray = new Npc[6];
    
    Array speakerList=Enum.GetValues(typeof(Speaker));
    var rnd = new Random();


    Npc jesus = Npc.Instantiate().WithPosition(jesusPosition);
    jesus.Who(Speaker.Layman,"Jesus");
    AddChild(jesus);
    
    for (int i = 0; i < npcArray.Length/2; i++)
    {
      Npc npc=Npc.Instantiate();
      npc.Who(Speaker.Skeleton,"");
      npc.Position = jesusPosition + (i + 4) * new Vector2(35,0);
      npc.SetDirection(-1);
      npcArray[i * 2] = npc;
      AddChild(npc);
      
      npc=Npc.Instantiate();
      npc.Who(Speaker.Skeleton,"");
      npc.Position = jesusPosition - (i + 4) * new Vector2(35,0);
      npc.SetDirection(1);
      npcArray[i * 2+1] = npc;
      AddChild(npc);
    }

    Npc rightPharisee = Npc.Instantiate();
    rightPharisee.Who(Speaker.Red_Merchant,"");
    rightPharisee.SetDirection(-1);
    rightPharisee.Position = rightPhariseePosition;
    AddChild(rightPharisee);
    
    for (int i = 0; i < 3; i++)
    {
      Npc pharisee = Npc.Instantiate();
      pharisee.Who(Speaker.Red_Merchant,"");
      pharisee.SetDirection(-1);
      pharisee.Position = rightPhariseePosition - new Vector2(100+i*45, 0);
      AddChild(pharisee);
    }


    Vector2 rightSoldierPosition = rightPhariseePosition - new Vector2(150 + 7 * 35, 0);

    var soldierMovement = new List<ICutsceneAction>();
    
    for (int i = 0; i < 6; i++)
    {
      var soldier=PathDeterminantNpc.Instantiate().UpdateMovement(new List<MovementAction>(),
        0, true, false);
      soldier.Who(Speaker.Red_Warrior,"");
      soldier.SetDirection(1);
      soldier.Position = rightSoldierPosition - new Vector2(25,0)*i;
      AddChild(soldier);
      soldierMovement.Add(new MovementCutsceneAction(soldier,new List<MovementAction>{new(-1000,0)},
        0,true,false,0));
    }

    
    var actions = new List<Tuple<SpeechAction, List<ICutsceneAction>>>
    {

      SimpleCutsceneObjective.DelayedActionGroupWithoutSpeech(2, new List<ICutsceneAction>
      {
        new InlineCutsceneAction(() =>
        {
          Flash.ToColor(new Color(1, 1, 1), 3);
          Flash.Initiate();
        })
      }),

      SimpleCutsceneObjective.DelayedActionGroupWithoutSpeech(3, new List<ICutsceneAction>
      {
        new PanCutsceneAction(jesusPosition),
        new InlineCutsceneAction(() =>
        {
          Flash.ToColor(new Color(1, 1, 1, 0), 1);
          Flash.Initiate();
        })
      }),

      new(new SpeechAction(narrator, new SpeechLine(Narrator.Identity,"He healed the sick and paralyzed, and performed countless" +
                                                                      " miracles and broke numerous cultural barriers."), 2),
        new List<ICutsceneAction>())
    };

    foreach(Npc n in npcArray)
    {
      actions.Add(SimpleCutsceneObjective.DelayedActionGroupWithoutSpeech(0.3f,
        new List<ICutsceneAction> { new InlineCutsceneAction(() =>
        {
          
          n.Who((Speaker)speakerList.GetValue(rnd.Next(0,speakerList.Length-1-1))!,"");
        }) }));
    }
    
    actions.AddRange(new []
    {
      
      SimpleCutsceneObjective.DelayedActionGroupWithoutSpeech(2, new List<ICutsceneAction>
      {
        new InlineCutsceneAction(() =>
        {
          Flash.ToColor(new Color(1, 0,0), 0.5f);
          Flash.ToColor(new Color(138/255f, 43/255f,226/255f,0), 0.5f);
          Flash.Initiate();
        })
      }),
      
      SimpleCutsceneObjective.DelayedActionGroupWithoutSpeech(0.5f, new List<ICutsceneAction>
      {
        new PanCutsceneAction(phariseeCutscenePosition)
      }),
      
      
      new Tuple<SpeechAction, List<ICutsceneAction>>(new SpeechAction(narrator,
          new SpeechLine(Narrator.Identity,"Because of this, the Pharisees and Jewish leaders were afraid of losing their political power" +
                                           " and that their evil may be put into the light, so they plotted to kill Jesus. "),1),
        new List<ICutsceneAction>()),
      
      new(new SpeechAction(narrator,new SpeechLine(phariseeIdentity,"GO! Find him and BRING HIM TO ME! >:V >:V"),1.5),
        new List<ICutsceneAction>()),
      
      new(new SpeechAction(narrator,new SpeechLine(romanSoldierIdentity,"Sir yes sir!"),0.2),soldierMovement),
      SimpleCutsceneObjective.DelayedActionGroupWithoutSpeech(0, new List<ICutsceneAction>
      {
        new InlineCutsceneAction(() =>
        {
          Flash.ToColor(new Color(0,0,0,0),0);
          Flash.ToSolid(5);
          Flash.Initiate();
        })
      }),
      
      new(new SpeechAction(narrator,
          new SpeechLine(nicodemusIdentity,"Wait, didn't He do like... nothing wrong?"),6),
        new List<ICutsceneAction>()
      ),
      
      new(new SpeechAction(narrator, 
          new SpeechLine(secondPhariseeIdentity,"Shut it. And close the door on your way out."),1),
        new List<ICutsceneAction>()
      ),
      
      SimpleCutsceneObjective.DelayedActionGroupWithoutSpeech(1.5f)
      
    });

    
    return new ObjectiveDisplayGroup(new List<IHasObjective> { new SimpleCutsceneObjective(actions)});
  }
  
}