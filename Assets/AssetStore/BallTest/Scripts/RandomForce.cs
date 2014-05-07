using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class RandomForce : MonoBehaviour
{
	public KeyCode key=KeyCode.Space;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(key) )
		{
			float _x = Random.Range(-15f,15f);
			float _y = Random.Range(-15f,15f);
			float _z = Random.Range(-15f,15f);
			this.rigidbody.AddRelativeForce(_x,_y,_z,ForceMode.Impulse);
		}
	}
}
