using UnityEngine;
using System.Collections;

public static class RendererEx 
{
	    public static void SetColor( this Renderer renderer, Color color, int MaterialIndex = 0 )
    {
        Material m = renderer.materials[MaterialIndex];
        if ( m.HasProperty("_TintColor") )
        {
            m.SetColor( "_TintColor", color );
        }
        else
        {
            m.color = color;
        }
    }

    public static Color GetColor( this Renderer renderer, int materialIndex = 0 )
    {
        Material m = renderer.materials[materialIndex];

        if ( m.HasProperty("_TintColor") )
        {
            return m.GetColor("_TintColor");
        }
        
        return m.color;
    }

    public static void SetAlpha( this Renderer renderer, float alpha, int materialIndex = 0 )
    {
        Material m = renderer.materials[materialIndex];

        if ( m.HasProperty("_TintColor") )
        {
            Color c = m.GetColor("_TintColor");
            c.a     = alpha;
            m.SetColor( "_TintColor", c );

            return;
        }
        else if ( m.HasProperty("_Color") )
        {
            Color c = m.color;
            c.a = alpha;
            m.color = c;

            return;
        }

        MeshFilter meshComponent = renderer.GetComponent<MeshFilter>();
        if ( meshComponent )
        {
            Color[] colors = meshComponent.mesh.colors;
            for( int i = 0; i < colors.Length; ++i )
            {
                colors[i].a = alpha;
            }
            meshComponent.mesh.colors = colors;
        }
    }

    public static float GetAlpha( this Renderer renderer, int materialIndex = 0 )
    {
        return GetColor( renderer, materialIndex ).a;
    }
}
