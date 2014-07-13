using UnityEngine;
using System;

public class AlphaLerper : TriggerAction
{
	public float 		StartingAlpha 		= 1;
	public bool 	    UseStartingAlpha 	= false;
	public float 		TargetAlpha 		= 1;
	public float   		TotalTime 			= 0.0f;
	public EasingType 	Easing    			= EasingType.Linear;
	public Renderer[] 	Renderers;
	public int 		    MaterialIndex = 0;
	
	private Lerper 		m_alphaLerper;
	private float[]		m_previousAlphas;
	
	protected override void TriggerStart ()
	{
		m_alphaLerper 	 = new Lerper(Easing);
		m_previousAlphas = new float[Renderers.Length];
	}
	
	protected override void TriggerUpdate () 
	{
		if (!m_alphaLerper.done)
		{
			UpdateAlpha();
		}
	}
	
	void UpdateAlpha()
	{
		m_alphaLerper.Update();

		for( int i = 0; i < Renderers.Length; ++i )
		{
			Renderer r = Renderers[i];
			if ( r && r.gameObject.active )
			{
				float alpha = MathfEx.Lerp( m_previousAlphas[i], TargetAlpha, m_alphaLerper.value );
				r.SetAlpha( alpha, MaterialIndex );
			}
		}
	}
	
	protected override void TriggerFire()
	{
		if ( UseStartingAlpha )
		{
			for( int i = 0; i < m_previousAlphas.Length; ++i )
			{
				m_previousAlphas[i] = StartingAlpha;
			}
		}
		else 
		{
			for( int i = 0; i < Renderers.Length; ++i )
			{
				Renderer r = Renderers[i];
				if ( r && r.gameObject.active )
				{
					m_previousAlphas[i] = r.GetAlpha( MaterialIndex );
				}
			}
		}

		m_alphaLerper.Begin( 0f, 1f, TotalTime );
		
		UpdateAlpha();
	}
}
