using UnityEngine;
using System.Collections;

public enum ParticleSystemState
{
	Play,
	Pause,
	Stop,
	Clear
}

public class SetParticleSystemState : TriggerAction 
{
	public ParticleSystem[] Particles;
	public ParticleSystemState State = ParticleSystemState.Play;

	protected override void TriggerFire ()
	{
		foreach( ParticleSystem ps in Particles )
		{
			if ( ps )
			{
				switch( State )
				{
					case ParticleSystemState.Play: ps.Play(); break;
					case ParticleSystemState.Pause: ps.Pause(); break;
					case ParticleSystemState.Stop: ps.Stop(); break;
					case ParticleSystemState.Clear: ps.Clear(); break;

					default: break;	 
				}
			}
		}
	}
}
