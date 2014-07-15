using UnityEngine;
using System.Collections;

public class FollowPath : TriggerAction
{
	public SplineFollower 			Follower;
	public HermiteSplineController 	PathToFollow;
	public bool						UseRotations = true;
	public bool						RebuildSpline;
	
	protected override void TriggerFire ()
	{	
		if (RebuildSpline)
		{
			PathToFollow.BuildSpline();
		}

		Follower.Follow( PathToFollow, null, UseRotations );
	}
}
