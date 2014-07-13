using UnityEngine;
using System.Collections;

public class ScaleObjectOverTime : TriggerAction
{
	public GameObject TargetObject;
	
	public EasingType EasingType;
	
	public Vector3	StartScale 	= Vector3.one;
	public Vector3 	EndScale 	= Vector3.one;
	public float 	TotalTime 	= 0f;
	public bool 	UseStartScale;
	public bool 	UseScalesRelativeToOriginal = true;
	
	private Lerper 		m_scaleLerper;
	private Transform	m_transform;
	private Vector3		m_previousScale;
	private Vector3		m_endScale;
	private float		m_totalTime;
	
	// Use this for initialization
	protected override void TriggerStart () 
	{
		m_transform	  	= TargetObject.transform;
		m_scaleLerper 	= new Lerper( EasingType );
		m_totalTime		= TotalTime;
		
		if ( UseScalesRelativeToOriginal )
		{
			StartScale.Scale( m_transform.GetScale() );
			EndScale.Scale  ( m_transform.GetScale() );
		}
		
		m_endScale		= EndScale;
	}
	
	// Update is called once per frame
	protected override void TriggerUpdate () 
	{
		if (!m_scaleLerper.done)
		{
			m_scaleLerper.Update();
			m_transform.SetScale( MathfEx.Lerp( m_previousScale, m_endScale, m_scaleLerper.value ) );
		}
	}
	
	protected override void TriggerFire ( )
	{
		if ( UseStartScale )
		{
			m_previousScale = StartScale;
		}
		else
		{
			m_previousScale = m_transform.GetScale();
		}
		m_scaleLerper.Begin( 0f, 1f, m_totalTime );
		m_transform.SetScale( MathfEx.Lerp( m_previousScale, m_endScale, m_scaleLerper.value ) );
	}
}
