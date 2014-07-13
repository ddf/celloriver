using UnityEngine;
using System.Reflection;

public class QuitApplication : TriggerAction
{
	protected override void TriggerFire()   
	{
		Debug.Log("Quitting Game.");
		Application.Quit();
	}	
}
