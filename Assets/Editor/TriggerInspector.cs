using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

[@CustomEditor (typeof(Trigger))]
public class TriggerInspector : Editor 
{
	public override void OnInspectorGUI()
	{
		if ( GUILayout.Button( "Open Editor") )
		{
			TriggerEditor.ShowWindow( target as Trigger );
		}

		DrawDefaultInspector();

		if ( GUI.changed )
		{
			EditorUtility.SetDirty( target );
		}
	}
}
