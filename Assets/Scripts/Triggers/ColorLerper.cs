using UnityEngine;
using System;

public class ColorLerper : TriggerAction 
{
	public Color 		StartingColor = Color.white;
	public bool 	    UseStartingColor = false;
	public Color 		TargetColor = Color.white;
	public float   		TotalTime = 0f;
	public EasingType 	Easing    = EasingType.Linear;
	public Renderer[] 	Renderers;
	public int 		    MaterialIndex = 0;
	
	private Lerper 		m_colorLerper;
	private Color 		m_previousObjectColor;
	private Color[]		m_previousColors;
	private Color		m_endColor;
	
	protected override void TriggerStart ()
	{
		m_colorLerper 	 = new Lerper(Easing);
		m_endColor		 = TargetColor;
		m_previousColors = new Color[Renderers.Length];
	}
	
	protected override void TriggerUpdate () 
	{
		if (!m_colorLerper.done)
		{
			UpdateColor();
		}
	}
	
	protected void UpdateColor()
	{
		m_colorLerper.Update();

		for( int i = 0; i < Renderers.Length; ++i )
		{
			Renderer r = Renderers[i];
			if ( r && r.gameObject.active )
			{
				Color color = Color.Lerp( m_previousColors[i], m_endColor, m_colorLerper.value );
				r.SetColor( color, MaterialIndex );
			}
		}
	}
	
	protected override void TriggerFire()
	{
		if ( UseStartingColor )
		{
			m_previousObjectColor = StartingColor;
			for( int i = 0; i < m_previousColors.Length; ++i )
			{
				m_previousColors[i] = StartingColor;
			}
		}
		else 
		{
			for( int i = 0; i < Renderers.Length; ++i )
			{
				Renderer r = Renderers[i];
				if ( r && r.gameObject.active )
				{
					m_previousColors[i] = r.GetColor( MaterialIndex );
				}
			}
		}

		m_colorLerper.Begin( 0f, 1f, TotalTime );
		
		UpdateColor();
	}
}
