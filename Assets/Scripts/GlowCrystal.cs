using UnityEngine;
using System.Collections;

public class GlowCrystal : MonoBehaviour 
{
	public byte Note;
	public Color GlowColor    = Color.red;
	public Color DimColor     = Color.red * 0.2f;
	public AnimationCurve GlowAnimation;

	Renderer m_renderer;
	float    m_animTime;
	float    m_animLength;

	// Use this for initialization
	void Start () 
	{
		m_renderer = renderer;
		m_renderer.SetColor( DimColor );

		m_animLength = GlowAnimation[ GlowAnimation.length - 1 ].time;

		EventManager.instance.AddListener<MidiReceived>( HandleMidi );
	}
	
	// Update is called once per frame
	void Update () 
	{	
		if ( m_animTime < m_animLength )
		{
			m_animTime += Time.deltaTime;

			Color c = Color.Lerp( DimColor, GlowColor, GlowAnimation.Evaluate(m_animTime) );
			m_renderer.SetColor( c );
		}
	}

	void HandleMidi( MidiReceived evt )
	{
		if ( evt.message.data1 == Note )
		{
			m_animTime = 0;
		}
	}
}
