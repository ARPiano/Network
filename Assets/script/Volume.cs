using UnityEngine;
using System.Collections;

public class Volume : MonoBehaviour {

	public GameObject network;
	ClientMain cm;
	bool findVolumeTarget;
	Vector3 previousAngles;
	GameObject soda;
	public float volume;

	void Start(){
		cm = network.GetComponent<ClientMain>();
		findVolumeTarget = false;
	
		soda = GameObject.Find("SodaCanKnob");
		previousAngles = soda.transform.eulerAngles;
		volume = AudioListener.volume;
	}

	void Update(){
		if(findVolumeTarget==false){
			return;
		}
		if(previousAngles.y == soda.transform.eulerAngles.y)
			return;
		
		if(previousAngles.y < soda.transform.eulerAngles.y)
		{
			AudioListener.volume += 0.01f;
			if(AudioListener.volume > 1.0)	AudioListener.volume = 1.0f;
			//print("Increased");
		}
		else
		{
			AudioListener.volume -= 0.01f;
			if(AudioListener.volume < 0.0)	AudioListener.volume = 0.0f;
			//print("Decreased");
		}
		previousAngles = soda.transform.eulerAngles;
		volume = AudioListener.volume;
		cm.SendInfoToServer("volume: "+volume); 
	}

	void OnVolumeLost(){
		findVolumeTarget = true;
		cm.SendInfoToServer("find volume");
		Debug.Log("Find Volume Cylinder Target");
	}

	void OnVolumeDetected(){
		findVolumeTarget = false;
		cm.SendInfoToServer("lost volume");
		Debug.Log("Lost Volume Cylinder Target");
	}
}
