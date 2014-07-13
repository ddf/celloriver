using UnityEngine;
using System.Collections;

public static class TransformEx
{

    public static void SetScale( this Transform transform, Vector3 scale )
    {
    	transform.localScale = scale;
    }

    public static Vector3 GetScale( this Transform transform )
	{

    	return transform.localScale;
    }
}
