using UnityEngine;
using System.Collections;

public class Rotator : TriggerAction 
{
	public Transform	TargetTransform;
	public Vector3 		TargetRotation = Vector3.forward;
	public float 		TotalTime = 0f;
	public bool 		UseRelativeRotation;
	public EasingType 	Easing;
	
	private Transform	m_transform;

	// used for absolute rotating
	private Lerper 		m_rotationLerper;
	private Quaternion	m_previousRotation;
	private Quaternion	m_targetRotation;

	// used for relative rotating
	private Vector3 	m_rotationRate;

	
	// Use this for initialization
	protected override void TriggerStart () 
	{
		m_transform	  		= TargetTransform;
		m_rotationLerper	= new Lerper(Easing);
		m_targetRotation	= Quaternion.Euler( TargetRotation );
	}
	
	// Update is called once per frame
	protected override void TriggerUpdate () 
	{
		if (!m_rotationLerper.done)
		{
			m_rotationLerper.Update();

			if ( !UseRelativeRotation )
			{
				m_transform.localRotation = Quaternion.Lerp( m_previousRotation, m_targetRotation, m_rotationLerper.value );
			}
			else
			{
				if ( m_rotationLerper.done )
				{
					m_transform.localRotation = m_targetRotation;
				}
				else
				{
					m_transform.Rotate( m_rotationRate * Time.deltaTime );
				}
			}
		}
	}
	
	protected override void TriggerFire ()
	{
		m_previousRotation = m_transform.localRotation;
		
		if (UseRelativeRotation)
		{
			m_targetRotation = m_previousRotation * (Quaternion.Euler( TargetRotation ));
			if ( TotalTime > 0 )
			{
				m_rotationRate = TargetRotation / TotalTime;
			}
		}
		else
		{
			m_targetRotation = Quaternion.Euler( TargetRotation );
		}
		
		m_rotationLerper.Begin( 0f, 1f, TotalTime );
		
		m_rotationLerper.Update();

		m_transform.localRotation = Quaternion.Lerp( m_previousRotation, m_targetRotation, m_rotationLerper.value );
	}
}
