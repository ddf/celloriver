using UnityEngine;
using System.Collections;

public class MidiReceived : IEvent
{
    public MidiMessage message { get; private set; }

    public MidiReceived( MidiMessage msg )
    {
        message = msg;
    }
}

public class MidiMessageDistributor : MonoBehaviour
{
    MidiReceiver receiver;

    void Start ()
    {
        receiver = FindObjectOfType (typeof(MidiReceiver)) as MidiReceiver;
    }

    void Update ()
    {
        while (!receiver.IsEmpty) 
        {
            var message = receiver.PopMessage ();
            if (message.status == 0x90 && message.data1 != 0 ) 
            {
                EventManager.instance.QueueEvent( new MidiReceived(message) );
            }
        }
    }
}
