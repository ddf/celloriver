using UnityEngine;
using System.Collections;

public class PlayAnimation : TriggerAction
{
	public Animation 	    TargetObject;
	public AnimationClip 	AnimateClip;
	public float 			SpeedFactor = 1;
	public float 			BlendTime   = 0;
	public bool 			Reversed    = false;
	
	
	protected override void TriggerFire ()
	{
		if ( Reversed )
		{
			TargetObject[ AnimateClip.name ].speed = -SpeedFactor;
			TargetObject[ AnimateClip.name ].time  = AnimateClip.length;
		}
		else 
		{
			TargetObject[ AnimateClip.name ].speed = SpeedFactor;
		}

		if ( BlendTime == 0 )
		{
			TargetObject.Play( AnimateClip.name );
		}
		else
		{
			TargetObject.CrossFade( AnimateClip.name, BlendTime );
		}
	}
}
