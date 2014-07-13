using UnityEngine;
using System.Collections;

public class TriggerOnMidiMessage : Trigger 
{
	public uint Note;

	// Use this for initialization
	protected override void Start()
	{
		base.Start();

		EventManager.instance.AddListener<MidiReceived>( HandleMidi );
	}
	
	void HandleMidi( MidiReceived evt )
	{
		print( "trigger received message " + evt.message.status + " with data " + evt.message.data1 + ", " + evt.message.data2 );
		if ( (uint)evt.message.data1 == Note )
		{
			Fire();
		}
	}
}
