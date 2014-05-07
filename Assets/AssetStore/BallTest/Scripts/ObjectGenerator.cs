using UnityEngine;
using System.Collections;

public class ObjectGenerator : MonoBehaviour
{
	public GameObject prefab;
	public Transform parent;
	public float peiod;
	public KeyCode key=KeyCode.None;
	private bool turnOn=false;

	// Use this for initialization
	void Start () {
		if( prefab==null ) enabled=false;
	}
	
	// Update is called once per frame
	void Update () {
		if( key!=KeyCode.None && Input.GetKey(key) )
		{
			ClonePrefab();
		}
	}
	void OnDisable()
	{
		StopAllCoroutines();
	}
	public void ClonePrefab()
	{
		if( turnOn ) return;
		StopCoroutine("_ClonePrefab");
		StartCoroutine("_ClonePrefab");
	}
	private IEnumerator _ClonePrefab()
	{
		turnOn=true;
		GameObject _obj = GameObject.Instantiate(prefab,this.transform.position,Quaternion.identity) as GameObject;
		if( parent!=null ) _obj.transform.parent = parent;
		yield return new WaitForSeconds(peiod);
		turnOn=false;
	}
}
