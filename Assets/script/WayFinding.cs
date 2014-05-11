using UnityEngine;
using System.Collections;

public class WayFinding : MonoBehaviour {

	bool findImageTarget;
	int samples = 16;
	public GameObject network;
	ClientMain cm;

	void Start () {
		findImageTarget = false;
		cm = network.GetComponent<ClientMain>();
	}

	void Update () {
		if(findImageTarget){
			ArrayList keys = new ArrayList();
			bool left_zero=false;
			string first;
			string last;
			Vector3[] leftEmitPoints = new Vector3[samples];
			Vector3[] rightEmitPoints = new Vector3[samples];

			for(int i=0;i<samples;i++){
				float ratio=((float)i)/((float)samples);
				leftEmitPoints[i]=new Vector3(0,ratio,0);
				rightEmitPoints[i]=new Vector3(1,ratio,0);
			}
			//Detect left boundary
			for(int i=0;i<leftEmitPoints.Length;i++){
				Ray ray=Camera.main.camera.ViewportPointToRay(leftEmitPoints[i]);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)){
					keys.Add(hit.transform.name);
				}
			}

			keys.Sort();
			if(keys.Count>0){
				first=(string)keys[0];
			}else{
				first="14";
				left_zero=true;
			}

			//Detect right boundary
			keys.Clear();
			for(int i=0;i<rightEmitPoints.Length;i++){
				Ray ray=Camera.main.camera.ViewportPointToRay(rightEmitPoints[i]);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)){
					keys.Add(hit.transform.name);
				}
			}

			keys.Sort();
			if(keys.Count>0){
				last=(string)keys[keys.Count-1];
			}else{
				if(left_zero){
					last="00";
					first="00";
				}else{
					last="01";
				}
			}
			//Send message
			if(cm!=null){
				string s=first+","+last;
				cm.SendInfoToServer("area:"+s);
			}

		}
	}

	void OnDetected(){
		findImageTarget = true;
	}

	void OnLost(){
		findImageTarget = false;
	}
}
