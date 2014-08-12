using UnityEngine;
using System.Collections;

public class GeomPlanes : MonoBehaviour 
{
	public byte Note;

	Renderer m_renderer;

	// Use this for initialization
	void Start () 
	{
		m_renderer = renderer;

		m_renderer.SetAlpha( 0 );

		EventManager.instance.AddListener<MidiReceived>( HandleMidi );
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void HandleMidi( MidiReceived evt )
	{
		if ( evt.message.data1 == Note )
		{
			float alpha = (float)evt.message.data2 / 127.0f;
			m_renderer.SetAlpha( alpha );
		}
	}
}
