using UnityEngine;
using System.Collections;

public class EnableBehaviour : TriggerAction
{
	public GameObject 	TargetObject;
	public string		ComponentName;
	public bool 		Enable;
	
	MonoBehaviour		m_behavior;
	
	protected override void TriggerStart ()
	{
		Component		m_component = TargetObject.GetComponent( ComponentName );
		if ( m_component is MonoBehaviour )
		{
			m_behavior = (MonoBehaviour)m_component;
		}
		else
		{
			Debug.LogWarning(ComponentName + " not found in " + TargetObject.name + " or not MonoBehavior.");
		}
	}
	
	protected override void TriggerFire ()
	{
		if ( m_behavior )
		{
			m_behavior.enabled = Enable;
		}
	}
}
