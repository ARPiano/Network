using UnityEngine;
using System.Collections;

// #define AUTOCREATE
namespace DevelopManager
{
	public class DevSingleTon<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T instance = null;
		public static T Instance
		{
			get
			{
				if( instance == null )
				{
					instance = GameObject.FindObjectOfType<T>();
					#if AUTOCREATE
					if( instance==null )
						instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
					#endif
				}
				return instance;
			}
		}
		public virtual void Awake()
		{
			if(instance!=null && instance.GetInstanceID()!=this.GetInstanceID() )
			{
				Debug.LogWarning("Mutiple instances of singleton "+ this.name, this);
			}
			else
			{
				instance=this as T;
			}
		}
	}
}