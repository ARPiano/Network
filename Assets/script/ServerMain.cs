using UnityEngine;
using System.Collections;

public class ServerMain : MonoBehaviour {

	/**
	 * Message sent by client
	 * */
	private string clientMsg = "";
	public Material blue;
	public Material white;
	public Material findVolume;
	public Material lostVolume;

	public GameObject volumeLabel;
	public GameObject volumeNumber;
	public GameObject volumeCylinder;
	
	// Use this for initialization
	void Start () {
		SendInfoToClient(ServerWelcome.appStart);
		changeColor("change to black");
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
				pressKey(someInfo);
			}else if(someInfo.StartsWith("release:")){
				releaseKey(someInfo);
			}else if(someInfo.StartsWith("area:")){
				changeArea(someInfo);
			}else if(someInfo.StartsWith("find volume")){
				changeColor("change to blue");
			}else if(someInfo.StartsWith("lost volume")){
				changeColor("change to black");
			}else if(someInfo.StartsWith("volume:")){
				displayVolume(someInfo);
			}
	}
}

	void displayVolume(string someInfo){
		int i = "volume:".Length;
		string volumeAndAngle = someInfo.Substring(i);
		String[] strs = volumeAndAngle.Split(",");
		string volume = strs[0];
		volume.Trim();
		string angle = strs[1];
		angle.Trim();
		volumeNumber.GetComponent<TextMesh>().text  = volume;
		Vector3 langle = volumeCylinder.transform.localEulerAngles;
		langle.y = float.Parse(angle.Substring(0,4));
		volumeCylinder.transform.localEulerAngles = langle; 
	}

	void pressKey(string someInfo){
		//press a key
		int i = "press:".Length;
		string key = someInfo.Substring(i);
		key.Trim();
		GameObject keyObj = GameObject.Find(key);
		if(keyObj!=null){
			Vector3 pos = keyObj.transform.localPosition;
			pos.z = 0.03f;
			keyObj.transform.localPosition = pos;
		}
	}
	
	void releaseKey(string someInfo){
		//release a key
		int i = "release:".Length;
		string key = someInfo.Substring(i);
		key.Trim();
		GameObject keyObj = GameObject.Find(key);
		if(keyObj!=null){
			Vector3 pos = keyObj.transform.localPosition;
			pos.z = 0.0f;
			keyObj.transform.localPosition = pos;
		}
	}
	
	void changeArea(string someInfo){
		//area
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
	
	void changeColor(string command){
		switch(command){
		case "change to blue":
			volumeLabel.GetComponent<TextMesh>().color = findVolume.color;
			volumeNumber.GetComponent<TextMesh>().color = findVolume.color;
			volumeCylinder.GetComponent<Renderer>().material = findVolume;
			break;
		case "change to black":
			volumeLabel.GetComponent<TextMesh>().color = lostVolume.color;
			volumeNumber.GetComponent<TextMesh>().color = lostVolume.color;
			volumeCylinder.GetComponent<Renderer>().material = lostVolume;
			break;
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

