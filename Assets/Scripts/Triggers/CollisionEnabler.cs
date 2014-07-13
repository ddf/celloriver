using UnityEngine;
using System;
using System.Collections;

public class CollisionEnabler : TriggerAction 
{
	public GameObject 	TargetObject;
	public GameObject[] TargetObjects;
	public bool 		Active = true;
	
	protected override void TriggerFire()
	{
		if ( TargetObject )
		{
			TargetObject.collider.enabled = Active;
		}
		
		Array.ForEach( TargetObjects, go => go.collider.enabled = Active );
	}
}
