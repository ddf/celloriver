using UnityEngine;
using System.Collections;

public class SplineFollower : MonoBehaviour 
{
	public bool					   UseRotations;

	HermiteSplineController 	   m_controller;
	public HermiteSplineController currentSplineController
	{ 
		get 
		{
			return m_controller;
		}

		set 
		{
			// if we have a spline and we are still the follower
			// then we need to remove ourselves from that spline
			if ( m_controller && m_controller.follower == this )
			{
				m_controller.follower = null;
			}

			m_controller = value;

			if ( m_controller )
			{
				m_controller.follower = this;
			}
		}
	}

	float m_splineDuration;
	float m_currentTime;

	Transform mTransform;
	public new Transform transform 
	{
		get
		{
			if ( !mTransform ) { mTransform = GetComponent<Transform>(); }

			return mTransform;
		}
	}

	public Vector3 position
	{
		get 
		{
			return transform.position;
		}

		set 
		{
			transform.position = value;
		}
	}

	public Quaternion rotation 
	{
		get 
		{
			return transform.rotation;
		}

		set 
		{
			transform.rotation = value;
		}
	}

	void Update()
	{
	}

	public void Follow( HermiteSplineController controller, OnEndCallback onEnd, bool bUseRotations )
	{
		if ( controller )
		{
			controller.StartFollower( this, onEnd, bUseRotations );
		}
		else 
		{
			currentSplineController = null;
		}
	}
}
