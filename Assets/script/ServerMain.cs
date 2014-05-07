using UnityEngine;
using System.Collections;

public class ServerMain : MonoBehaviour {

	/**
	 * Message sent by client
	 * */
	private string clientMsg = "";
	
	// Use this for initialization
	void Start () {
		SendInfoToClient(ServerWelcome.appStart);
	}

	void Update(){
		if(Network.connections.Length==0){
			Application.LoadLevel("Server_welcome");
		}
	}

	void OnPlayerConnected(NetworkPlayer player) {
		//AskClientForInfo(player);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player){
		Network.RemoveRPCs(player);
	}
	
	void AskClientForInfo(NetworkPlayer player) {
		//networkView.RPC("SetPlayerInfo", player, player);
	}
	
	[RPC]
	
	void ReceiveInfoFromClient(string someInfo) {	
		if(someInfo!=null){
			//receive info
			if(someInfo.StartsWith("press:")){
				//press a key
				int i = "press:".Length;
				string key = someInfo.Substring(i+1);
				key.Trim();
				GameObject keyObj = GameObject.Find(key);
				if(keyObj!=null){
					keyObj.transform.Translate(0.0f,0.0f,0.03f);
				}
			}else if(someInfo.StartsWith("release:")){
				//release a key
				int i = "release:".Length;
				string key = someInfo.Substring(i+1);
				key.Trim();
				GameObject keyObj = GameObject.Find(key);
				if(keyObj!=null){
					keyObj.transform.Translate(0.0f,0.0f,-0.03f);
				}
			}else if(someInfo.StartsWith("area:")){
				//area
			}
		}
	}
	
	[RPC]
	
	void SendInfoToClient(string info) {
		networkView.RPC("ReceiveInfoFromServer", RPCMode.Others, info);
	}
	
	[RPC]
	
	void SendInfoToServer() { }
	
	[RPC]
	
	void SetPlayerInfo(NetworkPlayer player) { }
	
	[RPC]
	
	void ReceiveInfoFromServer(string someInfo) { }
	
}

