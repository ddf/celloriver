using UnityEngine;
using System;
using System.Collections;

public class SetActiveRecursively : TriggerAction
{
	public GameObject[]   	TargetObjects;
	public bool 			Active;
	
	protected override void TriggerFire ()
	{
		Array.ForEach( TargetObjects, go => { if ( go ) go.SetActive(Active); } );
	}
}
