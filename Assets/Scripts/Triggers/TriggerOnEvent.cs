using UnityEngine;
using System.Collections;

public class TriggerOnEvent : Trigger, IEventListener
{
	[@HideInInspector]
	public string EventName;
	public int 	  EventNumber = 0;

	int m_eventNumber;

	// Use this for initialization
	protected override void Start() 
	{
		base.Start();

		if ( EventName != null )
		{
			EventManager.instance.AddListener( this, EventName );
		}
	}

	public void HandleEvent( IEvent evt )
	{
		++m_eventNumber;

		if ( EventNumber == 0 || m_eventNumber == EventNumber )
		{
			Fire();
		}
	}
}
