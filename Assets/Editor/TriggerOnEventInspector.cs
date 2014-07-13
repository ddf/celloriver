using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

[@CustomEditor (typeof(TriggerOnEvent))]
public class TriggerOnEventInspector : Editor 
{
	public override void OnInspectorGUI()
	{
		if ( GUILayout.Button( "Open Editor") )
		{
			TriggerEditor.ShowWindow( target as Trigger );
		}

		DrawDefaultInspector();

		TriggerOnEvent trigger = target as TriggerOnEvent;

		SetEventName( trigger );

		if ( GUI.changed )
		{
			EditorUtility.SetDirty( target );
		}
	}

	static public void SetEventName( TriggerOnEvent trigger )
	{
		IEnumerable<System.Type> AllEvents = Assembly.GetAssembly( typeof(IEvent) ).GetTypes().Where( t => t.IsSubclassOf(typeof(IEvent)) );

		if ( AllEvents != null )
		{
			List<string> eventNames = new List<string>();
			foreach( System.Type t in AllEvents )
			{
				eventNames.Add( t.FullName );
			}

			int currentSelection = eventNames.IndexOf( trigger.EventName );
			currentSelection = EditorGUILayout.Popup( "Event", currentSelection, eventNames.ToArray() );

			if ( currentSelection == -1 )
			{
				trigger.EventName = null;
			}
			else
			{
				trigger.EventName = eventNames[ currentSelection ];
			}
		}
	}
}
