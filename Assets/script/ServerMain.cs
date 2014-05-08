using UnityEngine;
using System.Collections;

public class ServerMain : MonoBehaviour {

	/**
	 * Message sent by client
	 * */
	private string clientMsg = "";
	public Material blue;
	public Material white;
	
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
		Debug.Log(someInfo);
		if(someInfo!=null){
			//receive info
			if(someInfo.StartsWith("press")){
				//press a key
				int i = "press:".Length;
				string key = someInfo.Substring(i);
				key.Trim();
				GameObject keyObj = GameObject.Find(key);
				if(keyObj!=null){
					Vector3 pos = keyObj.transform.localPosition;
					pos.z = 0.03f;
					keyObj.transform.localPosition = pos;
//					int n = keyObj.transform.childCount;
//					for(int j=0;j<n;j++){
//						GameObject child = keyObj.transform.GetChild(j).gameObject;
//						if(child.renderer!=null){
//							child.renderer.material = blue;
//						}
//					}
				}
			}else if(someInfo.StartsWith("release:")){
				//release a key
				int i = "release:".Length;
				string key = someInfo.Substring(i);
				key.Trim();
				GameObject keyObj = GameObject.Find(key);
				if(keyObj!=null){
					Vector3 pos = keyObj.transform.localPosition;
					pos.z = 0.0f;
					keyObj.transform.localPosition = pos;
//					int n = keyObj.transform.childCount;
//					for(int j=0;j<n;j++){
//						GameObject child = keyObj.transform.GetChild(j).gameObject;
//						if(child.renderer!=null){
//							child.renderer.material = white;
//						}
//					}
				}
			}else if(someInfo.StartsWith("area:")){
				//area
//				Debug.Log(someInfo);
				int  i = "area:".Length;
				string area = someInfo.Substring(i);
				string[] param = area.Split(',');
				int first = int.Parse(param[0]);
				int second = int.Parse(param[1]);
				int max = Mathf.Max(first,second);
				first = Mathf.Min(first,second);
				second = max;
				Debug.Log("first:"+first+",second:"+second);

				for(int k = 1;k<=14;k++){
					GameObject activeObject = GameObject.Find(k+"");
					if(activeObject!=null){
					int n = activeObject.transform.childCount;
					for(int j=0;j<n;j++){
						GameObject child = activeObject.transform.GetChild(j).gameObject;
						if(child.renderer!=null){
							if(k>=first && k<=second){
								child.renderer.material = blue;
							}else{
								child.renderer.material = white;
							}
						}
					}
				}
			}
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

