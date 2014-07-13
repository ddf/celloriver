using UnityEngine;
using System.Collections;

public class EmitParticleEmitter : TriggerAction
{
	public ParticleEmitter 	TargetParticleEmitter;
	
	protected override void TriggerFire ()
	{
		TargetParticleEmitter.Emit();
	}
}
