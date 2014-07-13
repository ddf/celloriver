using UnityEngine;
using System;
using System.Collections;

public class Accelerator : TriggerAction 
{
	public GameObject[] TargetObjects;
	public Vector3		AccelerationRate;
	
	Vector3 m_translateAmount;
	bool m_started;
	
	protected override void TriggerFire()
	{
		m_started = true;
	}
	
	protected override void TriggerUpdate()
	{
		if (m_started)
		{
			m_translateAmount += AccelerationRate * Time.deltaTime;
			Array.ForEach( TargetObjects, go => go.transform.position += m_translateAmount * Time.deltaTime );
		}
	}
}
