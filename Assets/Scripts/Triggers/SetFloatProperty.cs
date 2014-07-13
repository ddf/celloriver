using UnityEngine;
using System.Reflection;

public class SetFloatProperty : TriggerAction
{
	public Component[] 				Components;
	public string 					PropertyName;
	public float 					Value;

	PropertyInfo[]					m_properties;

	protected override void TriggerStart()
	{
		m_properties = new PropertyInfo[ Components.Length ];

		for( int i = 0; i < Components.Length; ++i )
		{
			System.Type type = Components[i].GetType();
			m_properties[i]	 = type.GetProperty( PropertyName );
		}
	}
	
	protected override void TriggerFire()   
	{
		for( int i = 0; i < m_properties.Length; ++i )
		{
			if ( m_properties[i] != null )
			{
				m_properties[i].SetValue( Components[i], Value, null );
			}
		}
	}	
}
