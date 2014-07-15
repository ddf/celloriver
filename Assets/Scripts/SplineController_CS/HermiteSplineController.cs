using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eOrientationMode { NODE = 0, TANGENT }

[AddComponentMenu("Splines/Spline Controller")]
public class HermiteSplineController : MonoBehaviour
{
	public GameObject 		SplineRoot;
	public Color	  		SplineColor = Color.red;
	public float 	  		Duration = 10;
	public eOrientationMode OrientationMode = eOrientationMode.NODE;
	public eWrapMode		WrapMode = eWrapMode.ONCE;
	public bool 			AutoClose = true;
	public bool 			HideOnExecute = true;
	public float 			Speed = 1;

	public SplineFollower 	Follower = null;
	public bool 			AutoStart = true;
	
	public float speed 
	{
		get 
		{
			return Speed;
		}

		set 
		{
			Speed = value;

			spline.speed = value;
		}
	}

	public SplineFollower follower 
	{
		get 
		{
			return Follower;
		}

		set 
		{
			Follower = value;

			spline.follower = value;

			// print( "Follower for " + name + " is now " + value );
		}
	}

	public SplineInterpolator spline 
	{
		get 
		{
			if ( mSplineInterp == null )
			{
				mSplineInterp = GetComponent<SplineInterpolator>();
			}

			return mSplineInterp;
		}
	}

	public bool previewing 
	{ 
		get 
		{ 
			return mbPreviewing; 
		} 
	}

	Transform 			mSplineInterpTransform;
	SplineInterpolator 	mSplineInterp;

	Transform[] 		mTransforms;
	bool				mbGizmo = true;
	bool 				mbPreviewing = false;

	void OnDrawGizmos()
	{
		if ( mbGizmo && !mbPreviewing )
		{
			Transform[] trans = mTransforms;

			if ( Application.isEditor && !Application.isPlaying )
			{
				trans = GetTransforms();

				if ( trans != null && trans.Length > 1 )
				{
					// print( "OnGizmo" );
					SetupSplineInterpolator(spline, trans);
					spline.StartInterpolation(null, false, WrapMode);
				}
			}
	
			Vector3 prevPos = trans[0].position;
			for (int c = 1; c <= 100; c++)
			{
				float currTime = c * Duration / 100;
				Vector3 currPos = spline.GetHermiteAtTime(currTime);
				//float mag = (currPos-prevPos).magnitude * 2;
				//Gizmos.color = new Color(SplineColor.r*mag, SplineColor.g * mag, SplineColor.b * mag, 1);
				Gizmos.color = SplineColor;
				Gizmos.DrawLine(prevPos, currPos);
				prevPos = currPos;
			}
		}
	}

#if UNITY_EDITOR
	public void Preview()
	{	
		Vector3 	initialPosition = Follower.position;
		Quaternion  initialRotation = Follower.rotation;
		eWrapMode   savedWrapMode   = WrapMode;
		mbPreviewing 				= true;
		WrapMode     				= eWrapMode.ONCE;
		spline.speed 				= Speed;

		print( "Initial position before preview is " + initialPosition + " and initial rotation is " + initialRotation );

		mTransforms = GetTransforms();
		SetupSplineInterpolator( spline, mTransforms );
		StartFollower
		( 
			Follower, 
			// on end callback
			() => 
			{ 
				mbPreviewing 					  = false;
				Follower.position 				  = initialPosition; 
				Follower.rotation 				  = initialRotation; 
				this.WrapMode 					  = savedWrapMode; 
				// Follower.currentSplineController  = null;
			}, 
			Follower.UseRotations 
		);
	}

	public void PreviewUpdate( float dt )
	{
		spline.speed = Speed;
		spline.PreviewUpdate( dt );
	}
#endif

	void Start()
	{
		BuildSpline();

		if ( AutoStart && Follower )
		{
			Follower.Follow( this, null, Follower.UseRotations );
		}
	}
	
	public void BuildSpline()
	{		
		spline.speed = Speed;
		
		mTransforms = GetTransforms();
		
		SetupSplineInterpolator( spline, mTransforms );
		
		if ( HideOnExecute )
		{
			mbGizmo = false;
			DisableTransforms();
		}
	}
		

	void SetupSplineInterpolator(SplineInterpolator interp, Transform[] trans)
	{
		interp.Reset( true );

		// mSplineInterp.SetExplicitMode();

		float step = (AutoClose) ? Duration / trans.Length : Duration / (trans.Length - 1);

		int c;
		for (c = 0; c < trans.Length; c++)
		{
			if (OrientationMode == eOrientationMode.NODE)
			{
				interp.AddPoint(trans[c].position, trans[c].rotation, step * c, new Vector2(0, 1));
			}
			else if (OrientationMode == eOrientationMode.TANGENT)
			{
				Quaternion rot;
				if (c != trans.Length - 1)
				{
					rot = Quaternion.LookRotation(trans[c + 1].position - trans[c].position, trans[c].up);
				}
				else if (AutoClose)
				{
					rot = Quaternion.LookRotation(trans[0].position - trans[c].position, trans[c].up);
				}
				else
				{
					rot = trans[c].rotation;
				}

				interp.AddPoint(trans[c].position, rot, step * c, new Vector2(0, 1));
			}
		}

		if (AutoClose)
		{
			interp.SetAutoCloseMode(step * c);
		}
	}


	/// <summary>
	/// Returns children transforms, sorted by name.
	/// </summary>
	Transform[] GetTransforms()
	{
		if (SplineRoot != null)
		{
			if ( mTransforms != null )
			{
				EnableTransforms();
			}
			
			Transform[] components = SplineRoot.GetComponentsInChildren<Transform>() as Transform[];
			List<Transform> transforms = new List<Transform>(); 
			
			foreach( Transform t in components )
			{
				if ( t.tag == "PathNode" )
				{
					transforms.Add( t );
				}
			}

			transforms.Sort( (Transform a, Transform b) => a.name.CompareTo(b.name) );

			return transforms.ToArray();
		}
		
		return null;
	}

	/// <summary>
	/// Disables the spline objects, we don't need them outside design-time.
	/// </summary>
	void DisableTransforms()
	{
		foreach( Transform t in mTransforms )
		{
			t.gameObject.SetActiveRecursively( false );
			t.renderer.enabled = false;
		}
		
//		if (SplineRoot != null)
//		{
//			SplineRoot.SetActiveRecursively(false);
//		}
	}
	
	/// <summary>
	/// Enabled the spline objects, we need them to calculate spline (like when rebuilding path).
	/// </summary>
	void EnableTransforms()
	{
		foreach( Transform t in mTransforms )
		{
			t.gameObject.SetActiveRecursively( true );
			t.renderer.enabled = true;
		}

	}
	

	/// <summary>
	/// Starts the interpolation
	/// </summary>
	public void StartFollower( SplineFollower newFollower, OnEndCallback onEnd, bool bRotations )
	{
		if ( mTransforms != null && mTransforms.Length > 0 )
		{
			spline.Reset( false );
			spline.StartInterpolation(onEnd, bRotations, WrapMode);
			spline.SnapToStart();

			// will set our follower
			newFollower.currentSplineController = this;
		}
		else // probably we aren't enabled yet, so make us start when enabled
		{
			// will set our follower
			newFollower.currentSplineController = this;
			AutoStart = true;
		}
	}
}