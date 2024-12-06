using System.Collections.Generic;
using SpiritualAdventure.entities;

namespace SpiritualAdventure.objects;

public class CutsceneMovementAction:DelayedCutsceneAction
{
  private List<MovementAction> moves;
  private float moveDelay;
  private bool isRelativePath,repeatMotion;
  private PathDeterminantNpc npc;
  public CutsceneMovementAction(PathDeterminantNpc npc,List<MovementAction> moves,float moveDelay,
    bool isRelativePath,bool repeatMotion,double initialDelay) : base(initialDelay)
  {
    this.moves = moves;
    this.moveDelay = moveDelay;
    this.isRelativePath = isRelativePath;
    this.repeatMotion = repeatMotion;
    this.npc = npc;
  }

  protected override void ActAfterDelay()
  {
    npc.UpdateMovement(moves,moveDelay, isRelativePath,repeatMotion);
    npc.SetMoving(true);
  }
}