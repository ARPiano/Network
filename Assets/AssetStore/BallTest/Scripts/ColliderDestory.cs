using UnityEngine;
using System.Collections;

public class ColliderDestory : MonoBehaviour {
	void OnTriggerEnter(Collider other)
	{
		Destroy(other.gameObject);
	}
	void OnTriggerStay(Collider other)
	{
		Destroy(other.gameObject);
	}
}
