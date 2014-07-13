using UnityEngine;
using System.Collections;

public class TriggerAction : MonoBehaviour
{
	public float		FireDelay;
	public bool 		TriggerOnce;
	
	float				m_fireDelayElapsed;
	bool				m_waitingForFire = false;
	int					m_triggerCount;

	public int triggerCount { get { return m_triggerCount; } }
	
	protected virtual void TriggerStart()  {}
	protected virtual void TriggerFire()   {}
	protected virtual void TriggerUpdate() {}
	
	void Start()
	{
		TriggerStart();
	}
	
	void Update()
	{
		TriggerUpdate();
		
		if ( m_waitingForFire )
		{
			if ( m_fireDelayElapsed >= FireDelay )
			{
				_Fire();
			}
			
			m_fireDelayElapsed += Time.deltaTime;
			
			if ( !m_waitingForFire )
			{
				m_fireDelayElapsed = 0;
			}
		}
	}

	void _Fire()
	{
		TriggerFire();
		m_waitingForFire = false;
	}
	
	public void Fire()
	{
		if (!TriggerOnce || m_triggerCount < 1)
		{
			m_waitingForFire = true;
			m_triggerCount++;
			m_fireDelayElapsed = 0;

			if ( FireDelay == 0 )
			{
				_Fire();
			}
		}
	}
}
