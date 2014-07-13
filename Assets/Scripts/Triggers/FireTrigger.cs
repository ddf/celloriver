using UnityEngine;
using System.Collections;

public class FireTrigger : TriggerAction
{
	public Trigger TriggerToFire;
	
	protected override void TriggerFire()   
	{
		TriggerToFire.Fire();
	}	
}
