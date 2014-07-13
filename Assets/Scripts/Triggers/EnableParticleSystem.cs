using UnityEngine;
using System.Collections;
using System;

public class EnableParticleSystem : TriggerAction
{
	public ParticleSystem 		TargetParticleSystem;
	public ParticleSystem[] 	TargetParticleSystems;
	public bool					Active;
	
	protected override void TriggerFire ()
	{
		if (Active)
		{
			if ( TargetParticleSystem && TargetParticleSystem.gameObject.active )
			{
				TargetParticleSystem.Play();
			}

			Array.ForEach( TargetParticleSystems, ps => { if ( ps && ps.gameObject.active ) ps.Play(); } );
		}
		else
		{
			if ( TargetParticleSystem )
			{
				TargetParticleSystem.Stop();
			}

			Array.ForEach( TargetParticleSystems, ps => { if ( ps ) ps.Stop(); } );
		}
	}
}
