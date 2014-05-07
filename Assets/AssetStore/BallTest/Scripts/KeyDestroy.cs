using UnityEngine;
using System.Collections;

public class KeyDestroy : MonoBehaviour {
	public KeyCode key = KeyCode.Delete;
	private bool killThem=false;

	void OnEnable()
	{
		iTween.Init(this.gameObject);
		Out ();
	}
	void OnDisable()
	{
		iTween.Stop();
	}

	void Update()
	{
		if( Input.GetKeyDown(key) )
		{
			Debug.Log ("Destroyer : starting.");
			killThem=true;
			In ();
			Rigidbody[] _obj = GameObject.FindObjectsOfType<Rigidbody>();
			Debug.Log("Destroyer found : "+ _obj.Length);
			for(int i=0; i<_obj.Length; i++)
			{
				if( _obj[i].rigidbody!=null && _obj[i].rigidbody.IsSleeping() )
				{
					_obj[i].rigidbody.WakeUp();
				}
			}
		}
		else if( Input.GetKeyUp(key) )
		{
			Debug.Log ("Destroyer : clear up finished.");
			killThem=false;
			Out ();
		}
	}

	void In()
	{
		Hashtable _ht = new Hashtable();
		_ht.Add("x",0f);
		_ht.Add("y",0f);
		_ht.Add("z",0f);
		_ht.Add("time",4f);
		iTween.Stop();
		iTween.MoveTo(this.gameObject, _ht);
	}
	void Out()
	{
		Hashtable _ht = new Hashtable();
		_ht.Add("x",-100f);
		_ht.Add("y",0f);
		_ht.Add("z",0f);
		_ht.Add("time",1f);
		iTween.Stop();
		iTween.MoveTo(this.gameObject, _ht);
	}

	void OnTriggerEnter(Collider other)
	{
		if( killThem ) Destroy(other.gameObject);
	}
	void OnTriggerStay(Collider other)
	{
		if( killThem ) Destroy(other.gameObject);
	}
	void OnTriggerExit(Collider other)
	{
		if( killThem ) Destroy(other.gameObject);
	}
}
