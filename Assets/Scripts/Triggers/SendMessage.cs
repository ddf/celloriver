using UnityEngine;
using System;

public class SendMessage : TriggerAction
{
	public GameObject[] TargetObjects;
	public string       Message;
	
	protected override void TriggerFire()   
	{
		Array.ForEach( TargetObjects, to => { if (to.active) to.SendMessage(Message); } );
	}	
}
