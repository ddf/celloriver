using UnityEngine;
using System.Collections;

public class AnimateCameraFOV : TriggerAction 
{
	public Camera 		TargetCamera;
	public float  		TargetFOV;
	public float 		Duration;
	public EasingType	Ease;

	Lerper m_lerper;

	protected override void TriggerStart()
	{
		 m_lerper = new Lerper( Ease );
	}

	protected override void TriggerFire()
	{
		float startAt = TargetCamera.orthographic ? TargetCamera.orthographicSize : TargetCamera.fieldOfView;
		m_lerper.Begin( startAt, TargetFOV, Duration );
	}

	protected override void TriggerUpdate()
	{
		if ( !m_lerper.done )
		{
			m_lerper.Update();

			if ( TargetCamera.orthographic )
			{
				TargetCamera.orthographicSize = m_lerper.value;
			}
			else 
			{
				TargetCamera.fieldOfView = m_lerper.value;
			}
		}
	}
	
}
