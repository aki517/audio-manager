using UnityEngine;

/// <summary>
/// シングルトンクラス.
/// </summary>
public class TSingleton<T> : MonoBehaviour where T : TSingleton<T>
{
	private static T m_instance = null;

	private void Awake()
	{
		if( m_instance != null ){
			Destroy( m_instance.gameObject );
			m_instance = null;
		}

		m_instance = ( T )this;

		Debug.Log( this.name + "is instantiated." );

		OnAwake();
	}

	protected virtual void OnAwake(){}

	/// <summary>
	/// 自身を削除.
	/// </summary>
	public virtual void DeleteThis(float deleteTime = 0.0f)
	{
		m_instance = null;
 		Destroy( this.gameObject, deleteTime );
	}
	public virtual void DeleteThisImmediately()
	{
		m_instance = null;
        DestroyImmediate( this.gameObject );
	}    
	
	public static T	I { get{ return m_instance; } }

}	// End of class TSingletonBehaviour<T>.

