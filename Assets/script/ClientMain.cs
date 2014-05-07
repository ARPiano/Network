using UnityEngine;
using System.Collections;

public class ClientMain : MonoBehaviour {

	private string infoSentToServer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.touchCount>0 || Input.GetMouseButton(0)){
			sendMsg("press C1");
		}
	}

	void sendMsg(string msg){
		infoSentToServer = msg;
		SendInfoToServer();
	}

	void SendInfoToServer(){
		networkView.RPC("ReceiveInfoFromClient", RPCMode.Server, infoSentToServer);
	}
	
}
