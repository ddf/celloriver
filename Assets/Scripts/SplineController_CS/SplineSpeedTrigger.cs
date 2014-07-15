using UnityEngine;
using System.Collections;

public class SplineSpeedTrigger : MonoBehaviour 
{
	public HermiteSplineController Controller;
	public float 				   Speed;
	public float 				   TransitionTime = 0.5f;

	Collider 				m_collider;
	bool 					m_bInside;
	Lerper					m_controllerSpeed;

	void Start()
	{
		if ( Controller == null )
		{
			Controller = transform.parent.GetComponent<HermiteSplineController>();
		}

		m_collider 	 = collider;

		m_controllerSpeed = new Lerper();
	}

	void Update()
	{
		if ( !m_controllerSpeed.done )
		{
			m_controllerSpeed.Update();
			
			Controller.speed = m_controllerSpeed.value;
		}

		bool bWasInside = m_bInside;

		m_bInside = m_collider.bounds.Contains( Controller.transform.position );

		if ( !bWasInside && m_bInside )
		{
			m_controllerSpeed.Begin( Controller.speed, Speed, TransitionTime );
		}
	}
}
