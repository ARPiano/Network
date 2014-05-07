using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ColliderPortal : MonoBehaviour
{

	public enum TYPE
	{
		In=0,
		Out,
		InAndOut
	}
	public TYPE type;
	public ColliderPortal linked;

	void OnTriggerEnter(Collider other)
	{
		if( linked!=null && type!=TYPE.Out )
		{
			other.transform.position = linked.transform.position;
		}
	}
	void OnTriggerStay(Collider other)
	{
		
	}
	void OnTriggerExit(Collider other){}
}
