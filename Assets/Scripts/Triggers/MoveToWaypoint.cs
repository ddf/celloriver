using UnityEngine;
using System.Collections;

public class MoveToWaypoint : TriggerAction
{
	public GameObject 	TargetObject;
	public GameObject 	Waypoint;
	
	public float		Seconds = 1f;

	public bool 		UseRotation = false;

	public EasingType   Easing = EasingType.Linear;
	
	Lerper				m_translationLerper;
	
	Vector3				m_oldPos;
	Quaternion  		m_oldRotation;
	
	
	// Use this for initialization
	protected override void TriggerStart () 
	{
		m_translationLerper = new Lerper( Easing );
	}
	
	protected override void TriggerFire()
	{
		m_oldPos 		= TargetObject.transform.position;
		m_oldRotation 	= TargetObject.transform.rotation;
			
		m_translationLerper.Begin( 0f, 1f, Seconds );
		
		m_translationLerper.Update();
		TargetObject.transform.position = MathfEx.Lerp( m_oldPos, Waypoint.transform.position, m_translationLerper.value );
		if ( UseRotation )
		{
			TargetObject.transform.rotation = Quaternion.Lerp( m_oldRotation, Waypoint.transform.rotation, m_translationLerper.value );
		}
	}
	
	protected override void TriggerUpdate ()
	{		
		if ( !m_translationLerper.done )
		{
			m_translationLerper.Update();
			TargetObject.transform.position = MathfEx.Lerp( m_oldPos, Waypoint.transform.position, m_translationLerper.value );
			if ( UseRotation )
			{
				TargetObject.transform.rotation = Quaternion.Lerp( m_oldRotation, Waypoint.transform.rotation, m_translationLerper.value );
			}
		}
	}
}
