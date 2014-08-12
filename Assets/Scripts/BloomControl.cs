using UnityEngine;
using System.Collections;

public class BloomControl : MonoBehaviour 
{
	public Bloom bloom;
	public float MinIntensity = 0.65f;
	public float MaxIntensity = 15.0f;
	public byte  LowNote;
	public byte  HighNote;

	float[] velocities;

	float m_accum;

	// Use this for initialization
	void Start () 
	{
		velocities = new float[HighNote-LowNote+1];

		EventManager.instance.AddListener<MidiReceived>( HandleMidi );
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_accum = 0;
		for ( int i = 0; i < velocities.Length; ++i )
		{
			m_accum += velocities[i];
		}

		bloom.bloomIntensity = MathfEx.Map( m_accum, 0, velocities.Length, MinIntensity, MaxIntensity );
	}

	void HandleMidi( MidiReceived evt )
	{
		if ( evt.message.data1 >= LowNote && evt.message.data1 <= HighNote )
		{
			float alpha = (float)evt.message.data2 / 127.0f;
			int ind = evt.message.data1 - LowNote;
			velocities[ind] = alpha;
		}
	}
}
