using UnityEngine;
using System.Reflection;

public class SetVector2Field : TriggerAction
{
	public Component[] 				Components;
	public string 					FieldName;
	public Vector2 					Value;

	FieldInfo[]						m_fields;

	protected override void TriggerStart()
	{
		m_fields = new FieldInfo[ Components.Length ];

		for( int i = 0; i < Components.Length; ++i )
		{
			System.Type type = Components[i].GetType();
			m_fields[i]	 	 = type.GetField( FieldName );
		}
	}
	
	protected override void TriggerFire()   
	{
		for( int i = 0; i < m_fields.Length; ++i )
		{
			if ( m_fields[i] != null )
			{
				m_fields[i].SetValue( Components[i], Value );
			}
		}
	}	
}
