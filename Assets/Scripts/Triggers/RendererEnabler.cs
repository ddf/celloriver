using UnityEngine;
using System.Collections;
using System;

public class RendererEnabler : TriggerAction
{
	public bool 		Active = true;
	public GameObject 	TargetObject;
	public Renderer[]   Renderers;

	protected override void TriggerFire ()
	{
		if ( TargetObject )
		{
			TargetObject.renderer.enabled = Active;
		}

		Array.ForEach( Renderers, r => { if ( r ) r.enabled = Active; } );
	}
}
