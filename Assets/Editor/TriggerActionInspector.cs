using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

[@CustomEditor (typeof(TriggerAction))]
public class TriggerActionInspector : Editor 
{
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if ( GUI.changed )
		{
			EditorUtility.SetDirty( target );
		}
	}
}
