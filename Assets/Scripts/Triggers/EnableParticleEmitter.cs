using UnityEngine;
using System.Collections;

public class EnableParticleEmitter : TriggerAction
{
	public ParticleEmitter 	TargetParticleEmitter;
	public bool				Active;
	
	protected override void TriggerFire ()
	{
		TargetParticleEmitter.emit = Active;
	}
}
