using UnityEngine;
using System.Collections;

public class Flash : TriggerAction
{
	public GameObject 		FlashObject;
	public Color 			FlashColor = Color.white;
	
	public float		SecondsTilWhite = 0.1f;
	public float 		SecondsOnWhite = 0.1f;
	public float		SecondsTilTransparent = 0.1f;

	Renderer 			m_flash;
	AnimationCurve      m_anim;
	float 				m_time;
	float 				m_totalTime;
	
	protected override void TriggerStart()
	{		
		m_flash = FlashObject.renderer;
		m_anim  = new AnimationCurve();

		if ( SecondsTilWhite <= 0 )
		{
			m_anim.AddKey( 0, 1 );
		}
		else
		{
			m_anim.AddKey( 0, 0 );
			m_anim.AddKey( SecondsTilWhite, 1 );
		}

		m_anim.AddKey( SecondsTilWhite + SecondsOnWhite, 1 );
		m_anim.AddKey( SecondsTilWhite + SecondsOnWhite + SecondsTilTransparent, 0 );

		m_time = m_totalTime = (SecondsTilWhite+SecondsOnWhite+SecondsTilTransparent);
	}
	
	protected override void TriggerFire ()
	{		
		m_flash.enabled = true;
		m_time = 0;

		m_flash.SetColor( FlashColor * m_anim.Evaluate(0) );
	}
	
	protected override void TriggerUpdate ()
	{
		if ( m_time < m_totalTime )
		{
			m_time += Time.deltaTime;

			m_flash.SetColor( FlashColor * m_anim.Evaluate(m_time) );
			
			if ( m_time >= m_totalTime )
			{
				m_flash.SetColor( Color.clear );
				m_flash.enabled = false;
			}
		}
	}
}
