using UnityEngine;
using System.Collections;

public class SpinAway : TriggerAction 
{
	public Transform TargetObject;
	public Vector3 	 Velocity;
	public bool 	 IsLocalVelocity = true;
	public Vector3 	 Angular;
	public bool 	 IsLocalAngular = true;
	public float 	 Duration;

	float m_activeTimer;

	protected override void TriggerFire()
	{
		m_activeTimer = Duration;
	}

	protected override void TriggerUpdate()
	{
		if ( m_activeTimer > 0 )
		{
			if ( IsLocalVelocity )
			{
				TargetObject.localPosition += Velocity * Time.deltaTime;
			}
			else
			{
				TargetObject.position += Velocity * Time.deltaTime;
			}

			if ( IsLocalAngular )
			{
				TargetObject.localEulerAngles += Angular * Time.deltaTime;
			}
			else 
			{
				TargetObject.eulerAngles += Angular * Time.deltaTime;
			}

			m_activeTimer -= Time.deltaTime;
		}
	}
}
