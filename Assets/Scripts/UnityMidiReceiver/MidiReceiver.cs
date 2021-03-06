﻿using UnityEngine;
using System.Collections.Generic;

// MIDI Receiver class.
public class MidiReceiver : MonoBehaviour
{
    Queue<MidiMessage> messageQueue;

    public bool IsEmpty {
        get { return messageQueue.Count == 0; }
    }
    
    public MidiMessage PopMessage ()
    {
        return messageQueue.Dequeue ();
    }

#if UNITY_EDITOR
    Queue<MidiMessage> messageHistory;

    public Queue<MidiMessage> History {
        get { return messageHistory; }
    }
#endif

    void Awake ()
    {
        messageQueue = new Queue<MidiMessage> ();
#if UNITY_EDITOR
        messageHistory = new Queue<MidiMessage> ();
#endif

        print( UnityMidiReceiver.GetError() );
    }

    void Update ()
    {
        while (true) {
            var data = UnityMidiReceiver.DequeueIncomingData ();
            if (data == 0) {
                break;
            }

            var message = new MidiMessage (data);

            if ( message.data2 != 0 )
            {
                messageQueue.Enqueue (message);
#if UNITY_EDITOR
                messageHistory.Enqueue (message);
#endif
            }
        }
#if UNITY_EDITOR
        while (messageHistory.Count > 64) {
            messageHistory.Dequeue ();
        }
#endif
    }
}