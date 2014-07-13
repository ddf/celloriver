using UnityEngine;
using System;

public class EnableBehaviours : TriggerAction
{
	public MonoBehaviour[] Behaviours;
	public bool 		   Enable;
	
	MonoBehaviour		m_behavior;
	
	protected override void TriggerStart ()
	{
	}
	
	protected override void TriggerFire ()
	{
		foreach( MonoBehaviour mb in Behaviours )
		{
			if ( mb )
			{
				mb.enabled = Enable;
			}
		}
	}
}
