using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eEndPointsMode { AUTO, AUTOCLOSED, EXPLICIT }
public enum eWrapMode { ONCE, LOOP, FREE }
public delegate void OnEndCallback();

public class SplineInterpolator : MonoBehaviour
{
	eEndPointsMode mEndPointsMode = eEndPointsMode.AUTO;

	internal class SplineNode
	{
		internal Vector3 	Point;
		internal Quaternion Rot;
		internal float 		Time;
		internal float      Length;	// what's the distance between this node and the next
		internal Vector2 	EaseIO;

		internal SplineNode(Vector3 p, Quaternion q, float t, Vector2 io) { Point = p; Rot = q; Time = t; EaseIO = io; }
		internal SplineNode(SplineNode o) { Point = o.Point; Rot = o.Rot; Time = o.Time; Length = o.Length; EaseIO = o.EaseIO; }
	}

	enum State 
	{
		Reset,
		Running,
		Looping,
		Freestyling,
		Stopped
	}

	List<SplineNode> 	mNodes = new List<SplineNode>();
	State 				mState;
	bool 				mRotations;
	bool 				mNeedSetInput;

	OnEndCallback mOnEndCallback;

	SplineFollower mFollower;
	public SplineFollower follower 
	{ 
		get
		{
			return mFollower;
		} 

		set
		{
			mFollower = value;
		}
	}

	public float currentTime 
	{
		get { return mCurrentTime; }

		set 
		{
			mCurrentTime = value;
		}
	}

	void Awake()
	{
		Reset( true );
	}
	
	void Start()
	{

	}

	// how quickly we should travel along the spline
	// -1 means this is disabled and spline will travel normally
	float  m_speed = -1;
	public float speed 
	{ 
		get { return m_speed; }

		set
		{
			m_speed = value;
		} 
	}
	
	public void SnapToStart()
	{
		if ( follower )
		{	
			follower.position = mNodes[0].Point;
			if ( mRotations )
			{
				follower.rotation = mNodes[0].Rot;
			}
		}
	}

	public void StartInterpolation(OnEndCallback endCallback, bool bRotations, eWrapMode mode)
	{
		if (mState != State.Reset)
		{
			throw new System.Exception("First reset, add points and then call here");
		}

		switch( mode )
		{
			case eWrapMode.ONCE: mState = State.Running; break;
			case eWrapMode.LOOP: mState = State.Looping; break;
			case eWrapMode.FREE: mState = State.Freestyling; break;
		}

		mRotations 		= bRotations;
		mOnEndCallback 	= endCallback;

		if ( mNeedSetInput )
		{
			SetInput();
		}
	}

	public void Reset( bool bClearNodes )
	{
		if ( bClearNodes )
		{
			mNodes.Clear();
			mNeedSetInput 	= true;
			mEndPointsMode 	= eEndPointsMode.AUTO;
		}
		mState = State.Reset;
		mCurrentIdx = 1;
		mCurrentTime = 0;
		mRotations = false;
	}

	public void AddPoint(Vector3 pos, Quaternion quat, float timeInSeconds, Vector2 easeInOut)
	{
		if (mState != State.Reset)
			throw new System.Exception("Cannot add points after start");

		SplineNode node = new SplineNode(pos, quat, timeInSeconds, easeInOut);

		mNodes.Add( node );
	}


	void SetInput()
	{
		if (mNodes.Count < 2)
			throw new System.Exception("Invalid number of points");

		if (mRotations)
		{
			for (int c = 1; c < mNodes.Count; c++)
			{
				SplineNode node = mNodes[c];
				SplineNode prevNode = mNodes[c - 1];

				// Always interpolate using the shortest path -> Selective negation
				if (Quaternion.Dot(node.Rot, prevNode.Rot) < 0)
				{
					node.Rot.x = -node.Rot.x;
					node.Rot.y = -node.Rot.y;
					node.Rot.z = -node.Rot.z;
					node.Rot.w = -node.Rot.w;
				}
			}
		}

		if (mEndPointsMode == eEndPointsMode.AUTO)
		{
			// print( "Inserting extra nodes for EndPointsMode.AUTO" );
			mNodes.Insert(0, mNodes[0]);
			mNodes.Add(mNodes[mNodes.Count - 1]);
		}
		else if (mEndPointsMode == eEndPointsMode.EXPLICIT && (mNodes.Count < 4))
		{
			throw new System.Exception("Invalid number of points");
		}

		CalculateDistances();

		mNeedSetInput = false;
	}

	public void SetExplicitMode()
	{
		if (mState != State.Reset)
			throw new System.Exception("Cannot change mode after start");

		mEndPointsMode = eEndPointsMode.EXPLICIT;
	}

	public void SetAutoCloseMode(float joiningPointTime)
	{
		if (mState != State.Reset)
			throw new System.Exception("Cannot change mode after start");

		mEndPointsMode = eEndPointsMode.AUTOCLOSED;

		mNodes.Add(new SplineNode(mNodes[0] as SplineNode));
		mNodes[mNodes.Count - 1].Time = joiningPointTime;

		Vector3 vInitDir = (mNodes[1].Point - mNodes[0].Point).normalized;
		Vector3 vEndDir = (mNodes[mNodes.Count - 2].Point - mNodes[mNodes.Count - 1].Point).normalized;
		float firstLength = (mNodes[1].Point - mNodes[0].Point).magnitude;
		float lastLength = (mNodes[mNodes.Count - 2].Point - mNodes[mNodes.Count - 1].Point).magnitude;

		SplineNode firstNode = new SplineNode(mNodes[0] as SplineNode);
		firstNode.Point = mNodes[0].Point + vEndDir * firstLength;

		SplineNode lastNode = new SplineNode(mNodes[mNodes.Count - 1] as SplineNode);
		lastNode.Point = mNodes[0].Point + vInitDir * lastLength;

		mNodes.Insert(0, firstNode);
		mNodes.Add(lastNode);
	}

	float 	mCurrentTime;
	int 	mCurrentIdx = 1;
	float   mDeltaTime;

#if UNITY_EDITOR
	public void PreviewUpdate( float dt )
	{
		mDeltaTime = dt;
		Update();
	}
#endif

	void Update()
	{
		if (mState == State.Reset || mState == State.Stopped || mNodes.Count < 4 || !follower )
			return;

		if ( Application.isPlaying ) 
		{
			mDeltaTime = Time.deltaTime;
		}

		if ( mState == State.Freestyling )
		{
			mCurrentIdx = GetIndexForTime(mCurrentTime);
			Vector3 newPos = new Vector3();
			GetPosition( ref newPos );
			follower.position = newPos;
		}
		else 
		{
			if ( m_speed != -1 )
			{
				float 	distanceToTravel = mDeltaTime * m_speed;
				float 	distanceTraveled = 0;
				Vector3	initialPosition  = follower.position;
				Vector3 nextPosition     = new Vector3();

				int iters = 0;
				while( distanceTraveled < distanceToTravel && mState != State.Stopped )
				{
					++iters;
					mCurrentTime += 0.0005f;
					GetPosition( ref nextPosition );
					distanceTraveled += Vector3.Distance( initialPosition, nextPosition );
					initialPosition = nextPosition;
				}
				// print( "SplineInterpolator update took " + iters + " iterations." );

				follower.position = initialPosition;
			}
			else
			{
				mCurrentTime += mDeltaTime;
				Vector3 newPos = new Vector3();
				GetPosition( ref newPos );
				follower.position = newPos;
			}
		}

		if ( mRotations && mState != State.Stopped )
		{
			// Calculates the t param between 0 and 1
			float param = 1.0f;
			
			// make sure we don't divide by 0
			if ( mNodes[mCurrentIdx+1].Time != mNodes[mCurrentIdx].Time )
			{
				param = (mCurrentTime - mNodes[mCurrentIdx].Time) / (mNodes[mCurrentIdx + 1].Time - mNodes[mCurrentIdx].Time);
			}
	
			// Smooth the param
			param = MathUtils.Ease(param, mNodes[mCurrentIdx].EaseIO.x, mNodes[mCurrentIdx].EaseIO.y);
	
			follower.rotation = GetSquad(mCurrentIdx, param);
		}

		// print( "SplineInterpolator current time is " + mCurrentTime + " and current index is " + mCurrentIdx );
	}

	void GetPosition( ref Vector3 position )
	{
		// We advance to next point in the path
		if (mState != State.Freestyling && mCurrentTime >= mNodes[mCurrentIdx + 1].Time)
		{
			if (mCurrentIdx < mNodes.Count - 3)
			{
				mCurrentIdx++;
			}
			else
			{
				if (mState != State.Looping)
				{
					mState = State.Stopped;

					// We stop right in the end point
					position = mNodes[mNodes.Count - 2].Point;

					if (mRotations)
						follower.rotation = mNodes[mNodes.Count - 2].Rot;

					// We call back to inform that we are ended
					if ( mOnEndCallback != null )
					{
						mOnEndCallback();
					}
				}
				else
				{
					mCurrentIdx = 1;
					mCurrentTime = 0;
				}
			}
		}

		if (mState != State.Stopped)
		{
			// Calculates the t param between 0 and 1
			float param = 1.0f;
			
			// make sure we don't divide by 0
			if ( mNodes[mCurrentIdx+1].Time != mNodes[mCurrentIdx].Time )
			{
				param = (mCurrentTime - mNodes[mCurrentIdx].Time) / (mNodes[mCurrentIdx + 1].Time - mNodes[mCurrentIdx].Time);
			}
	
			// Smooth the param
			param = MathUtils.Ease(param, mNodes[mCurrentIdx].EaseIO.x, mNodes[mCurrentIdx].EaseIO.y);
	
			position = GetHermiteInternal(mCurrentIdx, param);
		}	
	}


	// calculates the length of all the nodes
	public void CalculateDistances()
	{
		// print( "Calculating distances for " + mNodes.Count + " nodes." );
		for( int n = 1; n < mNodes.Count - 2; ++n )
		{
			SplineNode node  = mNodes[n];
			float timeslice  = 0.01f;
			Vector3 startPos = node.Point;
			for( float t = timeslice; t <= 1; t += timeslice )
			{
				Vector3 tpos = GetHermiteInternal( n, t );
				node.Length += Vector3.Distance( startPos, tpos );
				startPos = tpos;
			}

			// print( "Distance between node " + (n) + " and " + (n+1) + " is " + node.Length );
		}
	}
	
	Quaternion GetSquad(int idxFirstPoint, float t)
	{
		Quaternion Q0 = mNodes[idxFirstPoint - 1].Rot;
		Quaternion Q1 = mNodes[idxFirstPoint].Rot;
		Quaternion Q2 = mNodes[idxFirstPoint + 1].Rot;
		Quaternion Q3 = mNodes[idxFirstPoint + 2].Rot;

		Quaternion T1 = MathUtils.GetSquadIntermediate(Q0, Q1, Q2);
		Quaternion T2 = MathUtils.GetSquadIntermediate(Q1, Q2, Q3);

		return MathUtils.GetQuatSquad(t, Q1, Q2, T1, T2);
	}



	public Vector3 GetHermiteInternal(int idxFirstPoint, float t)
	{
		float t2 = t * t;
		float t3 = t2 * t;

		Vector3 P0 = mNodes[idxFirstPoint - 1].Point;
		Vector3 P1 = mNodes[idxFirstPoint].Point;
		Vector3 P2 = mNodes[idxFirstPoint + 1].Point;
		Vector3 P3 = mNodes[idxFirstPoint + 2].Point;

		float tension = 0.5f;	// 0.5 equivale a catmull-rom

		Vector3 T1 = tension * (P2 - P0);
		Vector3 T2 = tension * (P3 - P1);

		float Blend1 = 2 * t3 - 3 * t2 + 1;
		float Blend2 = -2 * t3 + 3 * t2;
		float Blend3 = t3 - 2 * t2 + t;
		float Blend4 = t3 - t2;

		return Blend1 * P1 + Blend2 * P2 + Blend3 * T1 + Blend4 * T2;
	}

	int GetIndexForTime( float timeParam )
	{
		int c;
		for (c = 1; c < mNodes.Count - 2; c++)
		{
			if (mNodes[c].Time > timeParam)
				break;
		}

		int idx = c - 1;

		return idx;
	}


	public Vector3 GetHermiteAtTime(float timeParam)
	{
		if (timeParam >= mNodes[mNodes.Count - 2].Time)
			return mNodes[mNodes.Count - 2].Point;

		int idx = GetIndexForTime(timeParam);
		float param = (timeParam - mNodes[idx].Time) / (mNodes[idx + 1].Time - mNodes[idx].Time);
		param = MathUtils.Ease(param, mNodes[idx].EaseIO.x, mNodes[idx].EaseIO.y);

		return GetHermiteInternal(idx, param);
	}
}