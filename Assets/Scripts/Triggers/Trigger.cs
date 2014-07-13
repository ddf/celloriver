using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Trigger : MonoBehaviour
{
	public float		FireDelay;
	public bool 		TriggerOnce;

	public enum FireType
	{
		AllAtOnce,
		Sequential,
	};

	public FireType 	TriggerType = FireType.AllAtOnce;
	
	protected float 			m_fireDelayElapsed;
	bool 						m_waitingForFire = false;
	int							m_triggerCount;
	
	[HideInInspector]
	public TriggerAction[]		TriggerActions = new TriggerAction[0];
	
#if UNITY_EDITOR
	bool						m_editorMaterialLoaded;
#endif
	
	protected virtual void Start () 
	{	
		if ( renderer )
		{
			renderer.enabled = false;
		}
	}
	
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if ( !Application.isPlaying && renderer )
		{
			if ( !m_editorMaterialLoaded )
			{
				renderer.enabled = true;
				
				renderer.sharedMaterial = new Material( Shader.Find("Transparent/Diffuse") );
				renderer.sharedMaterial.color = new Color(0.6f, 1f, 0, 0.3f);
				renderer.sharedMaterial.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath("Assets/Textures/Editor/Trigger.png", typeof(Texture2D)) as Texture2D);
				
				m_editorMaterialLoaded = true;
			}
		}
	}
#endif
	
	protected virtual void Update()
	{	
		if ( m_waitingForFire )
		{
			if ( m_fireDelayElapsed >= FireDelay )
			{
				switch( TriggerType )
				{
					case FireType.AllAtOnce:
					{
						foreach( TriggerAction ta in TriggerActions )
						{
							if ( ta && ta.enabled )
							{
								ta.Fire();
							}
						}
					}
					break;

					case FireType.Sequential:
					{
						int tid = (m_triggerCount-1) % TriggerActions.Length;
						{
							if ( TriggerActions[tid] && TriggerActions[tid].enabled )
							{
								TriggerActions[tid].Fire();
							}
						}
					}
					break;

					default:
					{
						Debug.LogWarning( "Trigger FireType " + TriggerType + " is not handled in Update!" );
					}
					break;
				}


				m_waitingForFire = false;
			}
			
			m_fireDelayElapsed += Time.deltaTime;
			
			if ( !m_waitingForFire )
			{
				m_fireDelayElapsed = 0;
			}
		}
	}
	
	public virtual void Fire()
	{
		if ( (!TriggerOnce || m_triggerCount < 1) )
		{
			// print( "Firing " + name );
			m_waitingForFire = true;
			m_triggerCount++;
		}
	}
}