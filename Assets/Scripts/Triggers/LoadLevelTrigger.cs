using UnityEngine;
using System;
using System.Collections;

public class LoadLevelTrigger : TriggerAction
{
	public string LevelName;
	
	protected override void TriggerFire ()
	{
		Application.LoadLevel(LevelName);
	}
}
