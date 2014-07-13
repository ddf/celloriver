using UnityEngine;
using System.Reflection;

public class MultiplyVector3Field : TriggerAction
{
	public Component[] 				Components;
	public string 					FieldName;
	public float 					Value;

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
				Vector3 fval = (Vector3)m_fields[i].GetValue( Components[i] );
				m_fields[i].SetValue( Components[i], fval*Value );
			}
		}
	}	
}
