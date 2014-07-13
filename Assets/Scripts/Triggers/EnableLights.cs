using UnityEngine;
using System;

public class EnableLights : TriggerAction
{
	public Light[] 			Lights;
	public bool 		   	Enable;
	
	protected override void TriggerStart ()
	{
	}
	
	protected override void TriggerFire ()
	{
		Array.ForEach( Lights, l => l.enabled = Enable );
	}
}
