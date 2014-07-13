using UnityEngine;
using System.Collections;
using System;

public class MaterialSwap : TriggerAction
{
	public GameObject 	TargetObject;
	public Renderer[]   TargetRenderers;
	public Material		NewMaterial;
	public int 			MaterialIndex = 0;
	
	protected override void TriggerFire ()
	{
		if ( TargetObject )
		{
			Material[] materials = TargetObject.renderer.materials;
			materials[MaterialIndex] = NewMaterial;
			TargetObject.renderer.materials = materials;
		}

		foreach( Renderer r in TargetRenderers )
		{
			Material[] mats = r.materials;
			mats[MaterialIndex] = NewMaterial;
			r.materials = mats;
		}
	}
}
