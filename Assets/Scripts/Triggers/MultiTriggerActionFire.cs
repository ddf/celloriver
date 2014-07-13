using UnityEngine;
using System.Collections;

public class MultiTriggerActionFire : TriggerAction
{
	public TriggerAction[] TriggerActions;
	
	protected override void TriggerFire()
	{
		foreach (TriggerAction ta in TriggerActions)
		{
			ta.Fire();
		}
	}
}
