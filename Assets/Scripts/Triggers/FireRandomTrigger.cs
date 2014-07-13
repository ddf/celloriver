using UnityEngine;
using System.Collections.Generic;

public class FireRandomTrigger : TriggerAction
{
	public List<Trigger> Triggers;
	
	protected override void TriggerFire()   
	{
		if (Triggers.Count > 0)
		{
			Triggers[Random.Range(0, Triggers.Count)].Fire();
		}
	}	
}
