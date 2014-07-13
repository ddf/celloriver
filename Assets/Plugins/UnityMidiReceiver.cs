using UnityEngine;
using System.Runtime.InteropServices;

// Bridge class for unity-midi-receiver plugin.
public static class UnityMidiReceiver
{
    [DllImport ("UnityMIDIReceiver", EntryPoint="UnityMIDIReceiver_CountEndpoints")]
    public static extern int CountEndpoints ();
    
    [DllImport ("UnityMIDIReceiver", EntryPoint="UnityMIDIReceiver_GetEndpointIDAtIndex")]
    public static extern uint GetEndpointIdAtIndex (int index);
    
    [DllImport ("UnityMIDIReceiver")]
    private static extern System.IntPtr UnityMIDIReceiver_GetEndpointName (uint id);
    
    [DllImport ("UnityMIDIReceiver", EntryPoint="UnityMIDIReceiver_DequeueIncomingData")]
    public static extern ulong DequeueIncomingData ();

    [DllImport ("UnityMIDIReceiver")]
    private static extern System.IntPtr UnityMIDIReceiver_GetError();

    public static string GetEndpointName (uint id)
    {
        return Marshal.PtrToStringAnsi (UnityMIDIReceiver_GetEndpointName (id));
    }

    public static string GetError()
    {
        return Marshal.PtrToStringAnsi( UnityMIDIReceiver_GetError() );
    }

    [DllImport ("UnityMIDIReceiver", EntryPoint="UnityMIDIReceiver_ReadProcHit")]
    public static extern bool ReadProcHit();
}
