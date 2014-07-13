using UnityEngine;
using System.Collections;

public static class MathfEx
{
	public static float Map( float value, float vlow, float vhigh, float tlow, float thigh )
	{
		return Mathf.Lerp( tlow, thigh, Mathf.InverseLerp( vlow, vhigh, value ) );
	}
	
    // unclamped version of lerping for Vector3
    public static Vector3 Lerp( Vector3 from, Vector3 to, float t )
    {
        Vector3 dir = to - from;
        return from + dir*t;
    }

    // unclamped version of lerp
    public static float Lerp( float from, float to, float t )
    {
        float dir = to - from;
        return from + dir*t;
    }
}
