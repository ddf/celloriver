using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

public class TriggerEditor : EditorWindow 
{
	public Texture2D ListItemBackground;

	Trigger 			trigger;
	SerializedObject 	serializedObject;
	Vector2 		scrollPosition 			 = Vector2.zero;
	// Vector2 		actionAreaScrollPosition = Vector2.zero;
	Vector2 	    actionListScrollPosition = Vector2.zero;
	string 			actionTypeToAdd			 = null;
	TriggerAction 	actionInSceneToAdd		 = null;
	TriggerAction   selectedAction 			 = null;

	bool 			dirty 		 			 = false;

	class ActionGUIData
	{
		public ActionGUIData( TriggerAction ta )
		{
			scrollPosition 		= new Vector2();
			serializedObject 	= new SerializedObject( ta );
			selected 			= false;
		}

		public Vector2  		scrollPosition;
		public bool 		    selected;
		public SerializedObject serializedObject;
	}

	Dictionary<TriggerAction, ActionGUIData> actionGUIData = new Dictionary<TriggerAction, ActionGUIData>();

	public static void ShowWindow( Trigger toEdit )
	{
		TriggerEditor ed 	= EditorWindow.GetWindow<TriggerEditor>();
		ed.trigger 		 	= toEdit;

		// do we already have a serialized object for this trigger?
		if ( ed.serializedObject != null && ed.serializedObject.targetObject == toEdit )
		{
			Debug.Log( "TriggerEditor already had a serializedObject for " + toEdit.ToString() );
		}
		else
		{
			ed.serializedObject = new SerializedObject(toEdit);
		}

		ed.actionGUIData.Clear();
		ed.selectedAction = null;
	}

	void OnEnable()
	{
		Debug.Log( "TriggerEditor enabled." );

		//ListItemBackground = EditorGUIUtility.FindTexture("Assets/Textures/SolidColors/purpleDark.tga");
		ListItemBackground   = AssetDatabase.LoadAssetAtPath( "Assets/Textures/SolidColors/purpleDark.tga", typeof(Texture2D) ) as Texture2D;

		if ( ListItemBackground == null )
		{
			Debug.Log( "Didn't find trigger editor image." );
		}
	}

	void AppendTriggerAction( TriggerAction ta )
	{
		// create some gui data for this
		ActionGUIData guiData = new ActionGUIData(ta);
		actionGUIData.Add( ta, guiData );

		// stick it on the end of the trigger action array
		SerializedProperty actions = serializedObject.FindProperty( "TriggerActions" );
		// can't use Insert when the array has a length of 0, so we have to set the array size to 1 first
		if ( actions.arraySize == 0 )
		{
			actions.arraySize = 1;
		}
		// otherwise, we just insert at the end
		else
		{
			actions.InsertArrayElementAtIndex( actions.arraySize - 1 );
		}

		// and then grab the last element
		SerializedProperty element 		= actions.GetArrayElementAtIndex( actions.arraySize - 1 );
		element.objectReferenceValue 	= ta;

		serializedObject.ApplyModifiedProperties();

		Debug.Log( "Appending " + ta.ToString() + " to the trigger actions list for " + trigger.ToString() + ". Actions list length is now " + trigger.TriggerActions.Length );

		dirty = true;
	}

	string ActionDisplayName( TriggerAction ta )
	{
		if ( ta )
		{
			return ta.name + " : " + ta.GetType().FullName;
		}

		return "";
	}

	void OnGUI()
	{
		if ( trigger == null ) 
		{
			Debug.Log( "TriggerEditor does not have a Trigger." );
			return;
		}

		if ( serializedObject == null )
		{
			Debug.Log( "TriggerEditor lost the SerializedObject and is making a new one." );
			serializedObject = new SerializedObject(trigger);
		}

		//Debug.Log( "Rendering TriggerEditor for Trigger " + trigger.ToString() + " with SerializedObject " + serializedObject.ToString() );

		float nodeWidth  = 350;
		float nodeHeight = 175;
		GUILayout.BeginArea( new Rect(10,10,nodeWidth,nodeHeight) );
		GUI.Box( new Rect(0,0,nodeWidth,nodeHeight), "" );
		scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, GUILayout.Height(nodeHeight) );

		// POPUP for choosing which trigger to edit from the editor
		{
			// find all objects of type Trigger
			Trigger[] allTriggers    = UnityEngine.Object.FindObjectsOfType( typeof(Trigger) ) as Trigger[];
			// sort alphabetically
			Array.Sort( allTriggers, delegate(Trigger trigger1, Trigger trigger2) 
			{
            	return trigger1.name.CompareTo( trigger2.name ); 
            });
			string[]  triggerNames 	 = new string[allTriggers.Length];
			int       selectedTrigger = -1 ;
			for( int t = 0; t < allTriggers.Length; ++t )
			{
				triggerNames[t] = allTriggers[t].name;
				if ( allTriggers[t] == trigger )
				{
					selectedTrigger = t;
				}
			}
			selectedTrigger = EditorGUILayout.Popup( selectedTrigger, triggerNames );
			if ( selectedTrigger >= 0 && allTriggers[selectedTrigger] != trigger )
			{
				trigger = allTriggers[selectedTrigger];
				serializedObject = new SerializedObject( trigger );
				actionGUIData.Clear();
			}
		}

		EditorGUILayout.InspectorTitlebar( false, trigger );
		{
			serializedObject.Update();

			EditorGUILayout.BeginHorizontal();
			{
				trigger.FireDelay = EditorGUILayout.FloatField( "Fire Delay", trigger.FireDelay, GUILayout.Width(185) );
			}
			EditorGUILayout.EndHorizontal();

			SerializedProperty properties = serializedObject.FindProperty( "TriggerOnce" );
			bool showChildren = false;
			do
			{
				showChildren = EditorGUILayout.PropertyField( properties );
			} 
			while( properties.NextVisible(showChildren) );

			if ( trigger is TriggerOnEvent )
			{
				TriggerOnEventInspector.SetEventName( trigger as TriggerOnEvent );
			}

			serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.EndScrollView();
		GUILayout.EndArea();

		GUILayout.BeginArea( new Rect(nodeWidth+20, 10, 200, 200) );
		if ( GUILayout.Button("Add TriggerAction Of Type") && actionTypeToAdd != null )
		{
			TriggerAction ta = trigger.gameObject.AddComponent( actionTypeToAdd ) as TriggerAction;
			AppendTriggerAction( ta );
		}

		System.Type triggerActionType = typeof(TriggerAction);
		IEnumerable<System.Type> AllTriggerActions = Assembly.GetAssembly( triggerActionType ).GetTypes().Where( t => t.IsSubclassOf(triggerActionType) );
		if ( AllTriggerActions != null )
		{
			List<string> actionNames = new List<string>();
			foreach( System.Type t in AllTriggerActions )
			{
				actionNames.Add( t.FullName );
			}

			int currentSelection = actionNames.IndexOf( actionTypeToAdd );
			currentSelection = EditorGUILayout.Popup( currentSelection, actionNames.ToArray() );

			if ( currentSelection == -1 )
			{
				actionTypeToAdd = null;
			}
			else
			{
				actionTypeToAdd = actionNames[ currentSelection ];
			}
		}
		else
		{
			Debug.LogError( "Didn't find any TriggerAction types!" );
		}

		GUILayout.Space( 20 );

		if ( GUILayout.Button("Add TriggerAction From Scene") && actionInSceneToAdd )
		{
			AppendTriggerAction( actionInSceneToAdd );
			actionInSceneToAdd = null;
		}

		TriggerAction[] allActions = FindObjectsOfType( typeof(TriggerAction) ) as TriggerAction[];
		if ( allActions.Length > 0 )
		{
			TriggerAction[] availableActions = Array.FindAll( allActions, ta => !trigger.TriggerActions.Contains(ta) );
			Array.Sort( availableActions, delegate(TriggerAction action1, TriggerAction action2) 
			{
	            return action1.name.CompareTo( action2.name ); 
	        });

			string[] actionNames = new string[availableActions.Length];

			int currentSelection = -1; 

			for( int a = 0; a < availableActions.Length; ++a )
			{
				actionNames[a] = ActionDisplayName( availableActions[a] );
				if ( actionNames[a] == ActionDisplayName( actionInSceneToAdd ) )
				{
					currentSelection = a;
				}
			}

			currentSelection = EditorGUILayout.Popup( currentSelection, actionNames );

			if ( currentSelection == -1 )
			{
				actionInSceneToAdd = null;
			}
			else
			{
				actionInSceneToAdd = availableActions[currentSelection];
			}
		}

		GUILayout.Space( 20 );

		if ( GUILayout.Button("Sort By Fire Delay") )
		{
			Array.Sort( trigger.TriggerActions, (ta1, ta2) => 
			{
				if ( ta1 == null && ta2 == null ) return 0;
				if ( ta1 == null ) return -1;
				if ( ta2 == null ) return 1;

				if ( ta1.FireDelay < ta2.FireDelay )
				{
					return -1;
				}

				if ( ta1.FireDelay > ta2.FireDelay )
				{
					return 1;
				}

				return 0;
			});
		}

		GUILayout.EndArea();


		// int totalTriggerActions  = trigger.TriggerActions.Length;
		int actionColumns 		 = 4;
		// int actionRows 			 = totalTriggerActions / actionColumns;
		int actionListItemHeight = 40;
		bool  needsCompact 	     = false;

		Rect actionListRect       = new Rect( 10, nodeHeight+20, nodeWidth, Screen.height - (nodeHeight+50) );
		GUILayout.BeginArea( actionListRect );
		actionListScrollPosition = EditorGUILayout.BeginScrollView( actionListScrollPosition, GUILayout.Height(actionListRect.height) );
		{
			SerializedProperty actions = serializedObject.FindProperty( "TriggerActions" );
			for( int i = 0; i < actions.arraySize; ++i )
			{
				TriggerAction 		action = actions.GetArrayElementAtIndex( i ).objectReferenceValue as TriggerAction;

				// skip null entries
				if ( action == null )
				{
					needsCompact = (trigger.TriggerType == Trigger.FireType.AllAtOnce);
					continue;
				} 

				ActionGUIData guiData = null;
				if ( actionGUIData.ContainsKey(action) )
				{
					guiData = actionGUIData[action];
				}
				else
				{
					guiData = new ActionGUIData( action );
					actionGUIData.Add( action, guiData );
				}

				SerializedObject 	so = guiData.serializedObject;

				so.Update();

				GUIStyle style = new GUIStyle( GUI.skin.box );
				if ( action == selectedAction )
				{
					style.normal.background = ListItemBackground;
				}

				EditorGUILayout.BeginVertical( style, GUILayout.Height(actionListItemHeight) );
				{
					// if already selected we don't allow unselecting
					if ( guiData.selected )
					{
						EditorGUILayout.InspectorTitlebar( true, action );						
					}
					// otherwise we can select to change the selection
					else if ( EditorGUILayout.InspectorTitlebar( false, action ) ) 
					{
						// find the current selection
						if ( selectedAction != null && actionGUIData.ContainsKey(selectedAction) )
						{
							ActionGUIData gd = actionGUIData[selectedAction];
							gd.selected = false;
						}
						guiData.selected = true;
					}

					if ( guiData.selected )
					{
						selectedAction = action;
					}

					EditorGUILayout.BeginHorizontal();
					{
						action.FireDelay = EditorGUILayout.FloatField( "Fire Delay", action.FireDelay, GUILayout.Width(185) );
					}
					EditorGUILayout.EndHorizontal();

					so.ApplyModifiedProperties();
				}
				EditorGUILayout.EndVertical();	
			}
		}
		EditorGUILayout.EndScrollView();
		GUILayout.EndArea();

		Rect triggerActionRect = new Rect(nodeWidth+20, nodeHeight+20, Screen.width - (nodeWidth+50), Screen.height - (nodeHeight+50));
		nodeHeight 			  *= 2;
		
		if ( selectedAction != null && actionGUIData.ContainsKey(selectedAction) )
		{
			ActionGUIData guiData = actionGUIData[selectedAction];

			SerializedObject so = guiData.serializedObject;
			so.Update();

			GUILayout.BeginArea( triggerActionRect );
			GUI.Box( new Rect(0,0,nodeWidth,nodeHeight), "" );
			guiData.scrollPosition = EditorGUILayout.BeginScrollView( guiData.scrollPosition, GUILayout.Height(nodeHeight), GUILayout.Width(nodeWidth) );
			EditorGUILayout.InspectorTitlebar( true, selectedAction );

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label( "Owned by " + selectedAction.name );
				if ( GUILayout.Button("Remove") )
				{
					for( int i = 0; i < trigger.TriggerActions.Length; ++i )
					{
						if ( trigger.TriggerActions[i] == selectedAction )
						{
							trigger.TriggerActions[i] = null;
						}
					}

					actionGUIData.Remove( selectedAction );
					selectedAction = null;
				}
			}
			EditorGUILayout.EndHorizontal();

			if ( selectedAction )
			{
				EditorGUILayout.BeginHorizontal();
				{
					selectedAction.FireDelay = EditorGUILayout.FloatField( "Fire Delay", selectedAction.FireDelay, GUILayout.Width(185) );
				}
				EditorGUILayout.EndHorizontal();

				// oh hey we don't have to do all the custom junk I wrote down there.
				{
					SerializedProperty properties = so.FindProperty( "TriggerOnce" );
					bool showChildren = false;
					do
					{
						showChildren = EditorGUILayout.PropertyField( properties );

					} while( properties.NextVisible(showChildren) );
				}
			}

			so.ApplyModifiedProperties();

			EditorGUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		bool bDrawAllActions = false;
		if ( bDrawAllActions )
		{
			float x = 10;
			float y = 0;
			needsCompact = false;
			SerializedProperty actions = serializedObject.FindProperty( "TriggerActions" );
			for( int i = 0; i < actions.arraySize; ++i )
			{
				TriggerAction 		ta = actions.GetArrayElementAtIndex( i ).objectReferenceValue as TriggerAction;

				// skip null entries
				if ( ta == null )
				{
					needsCompact = (trigger.TriggerType == Trigger.FireType.AllAtOnce);
					continue;
				} 

				ActionGUIData guiData = null;
				if ( actionGUIData.ContainsKey(ta) )
				{
					guiData = actionGUIData[ta];
				}
				else
				{
					guiData = new ActionGUIData( ta );
					actionGUIData.Add( ta, guiData );
				}

				SerializedObject 	so = guiData.serializedObject;

				if ( x > nodeWidth * actionColumns )
				{
					x = 10;
					y += nodeHeight + 10;
				}

				so.Update();

				GUILayout.BeginArea( new Rect(x,y,nodeWidth,nodeHeight) );
				GUI.Box( new Rect(0,0,nodeWidth,nodeHeight), "" );
				//GUILayout.BeginArea( new Rect(10,10,nodeWidth-20,nodeHeight-20) );
				guiData.scrollPosition = EditorGUILayout.BeginScrollView( guiData.scrollPosition, GUILayout.Height(nodeHeight) );
				EditorGUILayout.InspectorTitlebar( false, ta );

				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( "Owned by " + ta.name );
				if ( GUILayout.Button("Remove") )
				{
					trigger.TriggerActions[i] = null;
				}
				EditorGUILayout.EndHorizontal();

				// oh hey we don't have to do all the custom junk I wrote down there.
				{
					SerializedProperty properties = so.FindProperty( "FireDelay" );
					bool showChildren = false;
					do
					{
						showChildren = EditorGUILayout.PropertyField( properties );

					} while( properties.NextVisible(showChildren) );
				}

				so.ApplyModifiedProperties();

				EditorGUILayout.EndScrollView();
				GUILayout.EndArea();

				x = (x + nodeWidth + 20);
			}
		}

		if ( needsCompact )
		{
			trigger.TriggerActions = Array.FindAll( trigger.TriggerActions, ta => ta != null );
			dirty = true;
		}

		if ( GUI.changed || dirty )
		{
			dirty = false;
			EditorUtility.SetDirty( trigger );
		}
	}

/** how we tried drawing mini inspectors for TriggerActions at first
			
			{
				System.Type actionType 		= ta.GetType();
				FieldInfo[] publicFields 	= actionType.GetFields();
				foreach( FieldInfo field in publicFields )
				{
					System.Object value = field.GetValue( ta );
					if ( value is Array )
					{
						Array 		array 		= (Array)value;
						System.Type elementType = array.GetType().GetElementType(); 

						actionGUIData[i].arrayFoldout = EditorGUILayout.Foldout( actionGUIData[i].arrayFoldout, field.Name );

						// if the mouse is over this foldout, see if DragAndDrop contains objects we can add to the array.
						if( Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) ) 
						{
        					foreach( UnityEngine.Object o in DragAndDrop.objectReferences )
        					{
        						if ( o.GetType() == elementType )
        						{
        							DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        							break;
        						}
        					}
    					}

						if ( actionGUIData[i].arrayFoldout )
						{
							int newLength 		  	= EditorGUILayout.IntField( "     Size", array.Length );
							
							if ( newLength != array.Length )
							{
								Array copy = Array.CreateInstance( elementType, newLength );
								for( int idx = 0; idx < newLength && idx < array.Length; ++idx )
								{
									copy.SetValue( array.GetValue(idx), idx );
								}

								array = copy;
								value = copy;
							}

							for( int e = 0; e < array.Length; ++e )
							{
								array.SetValue( GUIForObject( "     Element " + i, array.GetValue(e), elementType ), e );
							}
						}
					}
					else
					{
						value = GUIForObject( field.Name, value, field.FieldType );
					}

					field.SetValue( ta, value );
				}
			}

	System.Object GUIForObject( string name, System.Object value, System.Type type )
	{
		if ( type == typeof(float) )
		{
			return (System.Object)EditorGUILayout.FloatField( name, (float)value );
		}
		else if ( type == typeof(int) )
		{
			return (System.Object)EditorGUILayout.IntField( name, (int)value );
		}
		else if ( type.IsEnum )
		{
			return (System.Object)EditorGUILayout.EnumPopup( name, (System.Enum)value );
		}
		else if ( type == typeof(bool) )
		{
			return (System.Object)EditorGUILayout.Toggle( name, (bool)value );
		}
		else if ( type == typeof(Vector2) )
		{
			return (System.Object)EditorGUILayout.Vector2Field( name, (Vector2)value );
		}
		else if ( type == typeof(Vector3) )
		{
			return (System.Object)EditorGUILayout.Vector3Field( name, (Vector3)value );
		}
		else if ( type == typeof(string) )
		{
			return (System.Object)EditorGUILayout.TextField( name, (string)value );
		}
		else if ( type == typeof(Color) )
		{
			return (System.Object)EditorGUILayout.ColorField( name, (Color)value );
		}
		else if ( type.IsSubclassOf( typeof(UnityEngine.Object) ) )
		{
			return (System.Object)EditorGUILayout.ObjectField( name, (UnityEngine.Object)value, type, allowSceneObjects );
		}

		return null;
	}
	**/

}
