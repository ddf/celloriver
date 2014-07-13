using UnityEngine;
using System;

public class SetLightProperties : TriggerAction 
{
	public Light[] 		Lights;
	
	public Color 		LightColor;
	public float		Intensity;
	
	
	protected override void TriggerStart ()
	{
	}
	
	protected override void TriggerUpdate () 
	{
	}

	protected override void TriggerFire()
	{
		foreach ( Light l in Lights )
		{
			l.color 		= LightColor;
			l.intensity 	= Intensity;
		}
	}
}
