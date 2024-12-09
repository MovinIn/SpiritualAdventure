using Godot;
using System;
using System.Collections.Generic;

public partial class Flash : ColorRect
{

  public interface IFlashEffect
  {
	public void StartEffect();
  
	/**
	 * proportion: a number in [0,1], always increasing; representing the current cumulative level of the effect.
	 */
	public void UpdateEffect(float proportion);
  }
  
  public class InlineFlashEffect:IFlashEffect
  {
	private Action startEffect;
	private Action<float> flashEffect;

	public InlineFlashEffect SetStartEffect(Action startEffect)
	{
	  this.startEffect = startEffect;
	  return this;
	}
  
	public InlineFlashEffect SetFlashEffect(Action<float> flashEffect)
	{
	  this.flashEffect = flashEffect;
	  return this;
	}
  
	public void StartEffect()
	{
	  startEffect?.Invoke();
	}

	public void UpdateEffect(float proportion)
	{
	  flashEffect?.Invoke(proportion);
	}
  }
  
  
  
  private static Flash singleton;

  private static bool isFlashing;

  private static double colorChangeDuration, stagnantDuration, dissolveDuration;

  private static double currTime;

  private static Queue<Tuple<IFlashEffect,double>> changes;

  public Flash()
  {
    singleton = this;
    isFlashing = false;
    changes = new Queue<Tuple<IFlashEffect, double>>();
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta)
  {
    if (!isFlashing) return;
    
	if (changes.Count == 0)
	{
	  currTime = 0;
	  isFlashing = false;
	  return;
	}

	currTime += delta;

	if(currTime<changes.Peek().Item2)
	{
	  changes.Peek().Item1.UpdateEffect((float)(currTime/changes.Peek().Item2));
	}
	
	while (currTime >= changes.Peek().Item2)
	{
	  currTime -= changes.Peek().Item2;
	  changes.Dequeue().Item1.UpdateEffect(1);
	  
	  if (changes.Count != 0)
	  {
		changes.Peek().Item1.StartEffect();
	  }
	  else
	  {
		return;
	  }
	}
  }

  public static void Set(Color c)
  {
	singleton.Modulate = c;
  }

  public static void Initiate()
  {
	if (changes.Count != 0)
	{
	  changes.Peek().Item1.StartEffect();
	}
	
	isFlashing = true;
  }

  public static void ToColor(Color endColor,double duration)
  {
	float dr=0,dg=0,db=0,da=0;
	Color baseColor = default;
	
	changes.Enqueue(new Tuple<IFlashEffect, double>(new InlineFlashEffect()
	  .SetStartEffect(() =>
	  {
		baseColor = singleton.Modulate;
		dr = endColor.R - baseColor.R;
		dg = endColor.G - baseColor.G;
		db = endColor.B - baseColor.B;
		da = endColor.A - baseColor.A;
	  })
	  
	  .SetFlashEffect(proportion =>
      {
		singleton.Modulate = new Color(baseColor.R+dr*proportion,baseColor.G+dg*proportion,
		  baseColor.B+db*proportion,baseColor.A+da*proportion);

      }),duration));
  }

  public static void StayStagnant(double duration)
  {
	changes.Enqueue(new Tuple<IFlashEffect, double>(new InlineFlashEffect(),duration));
  }

  public static void Dissolve(double duration)
  {
	IntegerizeAlpha(duration,true);
  }

  public static void ToSolid(double duration)
  {
	IntegerizeAlpha(duration,false);
  }
  
  private static void IntegerizeAlpha(double duration,bool toNothing)
  {
	float a=0;
	
	changes.Enqueue(new Tuple<IFlashEffect, double>(new InlineFlashEffect()
	  .SetStartEffect(() =>
	  {
		a = singleton.Modulate.A;
	  })
	  
	  .SetFlashEffect(proportion =>
	  {
		float newA = toNothing ? (1 - proportion) * a : a + (1 - a) * proportion;
		singleton.Modulate=new Color(singleton.Modulate,newA);
	  }),duration));
  }

  /**
   * Resets the color to transparent black.
   */
  public static void QueueReset()
  {
	changes.Enqueue(new Tuple<IFlashEffect, double>(new InlineFlashEffect().SetStartEffect(() =>
	{
	  singleton.Modulate = new Color(0,0,0,0);
	}),0));
  }
  
  /**
   * Clears all queued changes. Resets the color to transparent black.  
   */
  public static void HardInstantReset()
  {
	changes.Clear();
	singleton.Modulate = new Color(0,0,0,0);
	isFlashing = false;
  }
}
