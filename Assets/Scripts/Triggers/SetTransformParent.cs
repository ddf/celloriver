using UnityEngine;
using System.Collections;

public class SetTransformParent : TriggerAction 
{
	public Transform 	ParentTransform;
	public Transform[] 	Children;

	protected override void TriggerFire()
	{
		foreach( Transform t in Children )
		{
			if ( t ) t.parent = ParentTransform;
		}
	}
}
