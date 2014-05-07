using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class AutoForce : MonoBehaviour
{

	// Use this for initialization
	void OnEnable ()
	{
		StartCoroutine("_NewDirection");
	}
	void OnDisiable()
	{
		StopAllCoroutines();
	}
	public float power=10f;
	public ForceMode force = ForceMode.Impulse;
	public float changeTime=1f;
	public float changeTimeRange=3f;
	// Update is called once per frame
	public IEnumerator _NewDirection()
	{
		while( true )
		{
			float _x = Random.Range(-power,power);
			float _y = Random.Range(-power,power);
			float _z = Random.Range(-power,power);
			this.rigidbody.AddRelativeForce(_x,_y,_z,force);
			yield return new WaitForSeconds(Random.Range(changeTime,changeTimeRange));
		}
	}
}
