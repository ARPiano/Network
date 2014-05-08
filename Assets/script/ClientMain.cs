using UnityEngine;
using System.Collections;

public class ClientMain : MonoBehaviour {

	private string infoSentToServer;
		
	[RPC]
	
	void ReceiveInfoFromClient(string someInfo) {	
	}
	
	[RPC]
	
	void SendInfoToClient() {

	}
	
	[RPC]
	
	public void SendInfoToServer(string info) {
		Debug.Log("trying to send information:"+info);
		networkView.RPC("ReceiveInfoFromClient", RPCMode.Server, info);
	}
	
	[RPC]
	
	void SetPlayerInfo(NetworkPlayer player) { }
	
	[RPC]
	
	void ReceiveInfoFromServer(string someInfo) { }
	
}
