using UnityEngine;
using System.Collections;

public class TriggerOnMidiNote : Trigger
{
	public byte Note;

	// Use this for initialization
	protected override void Start()
	{
		base.Start();

		EventManager.instance.AddListener<MidiReceived>( HandleMidi );
	}
	
	void HandleMidi( MidiReceived evt )
	{
		if ( evt.message.data1 == Note )
		{
			Fire();
		}
	}
}
