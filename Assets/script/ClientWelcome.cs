using UnityEngine;
using System.Collections;

public class ClientWelcome : MonoBehaviour {

	public GUISkin skin;
			
	/**
	 * Some constants used for gui components
	 * */
	private Vector2 scrollPosition;
	private Rect windowRect = new Rect (10, 10, Screen.width-20, Screen.height-20);
	private string refreshServerLabel = "Refresh Server List";
	private string cancelLabel = "Cancel";
	private string quitLabel = "Quit";
	private string title = ServerWelcome.appName+" - iPone";
	
	/**
	 * Some constant messages
	 * */
	private string pleaseRefresh = "Press Refresh Server List.";
	private string serverFound = " display device(s).";
	public static string playerSet = "player set.";
	private string error_connection = "Unable to connect to server.";

	private string serverInfo;
	private string infoSentToServer;
	private string status = "Press Refresh Server List.";

	void OnStart(){
		display(pleaseRefresh);
	}

	void display(string msg){
		status = msg;
	}

	void OnGUI () {
		if(skin!=null){
			GUI.skin = skin;
		}
		windowRect = GUI.Window (0, windowRect, WindowFunction, "");
	}
	
	void WindowFunction (int windowID) {
		GUILayout.Label(title);
		if(!Network.isClient && MasterServer.PollHostList().Length>0){
			GUILayout.Box(MasterServer.PollHostList().Length+serverFound);
		}
		GUILayout.Box(status);
		if(!Network.isClient){
			if(GUILayout.Button(refreshServerLabel)){
				MasterServer.RequestHostList(ServerWelcome.appName);
			}
			foreach(HostData server in MasterServer.PollHostList()){
				if(GUILayout.Button(server.gameName)){
					Network.Connect(server);
					display("Try to connect to "+server.gameName+"...");
				}
			}
		}else{
			if(GUILayout.Button(cancelLabel)){
				Network.Disconnect();
			}
		}
		if(GUILayout.Button(quitLabel)){
			Application.Quit();
		}
	}

	[RPC]
	
	void SendInfoToServer(){
		
	}
	
	[RPC]
	
	void SetPlayerInfo(NetworkPlayer player) {
		infoSentToServer = playerSet;
		networkView.RPC("ReceiveInfoFromClient", RPCMode.Server, infoSentToServer);
	}
	
	
	
	[RPC]
	
	void ReceiveInfoFromServer(string someInfo) {
		Debug.Log(someInfo);
		display("receive from server:"+someInfo);
//		if(someInfo!=null && someInfo==ServerWelcome.appStart){
//			Application.LoadLevel("Client_main");
//		}
	}

	void OnConnectedToServer() {
		display("connected to server");
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		display("Disconnected.");
	}

	void OnFailedToConnect(NetworkConnectionError error) {
		display("Could not connect to server: " + error);
	
	}

	[RPC]
	
	void ReceiveInfoFromClient(string someInfo) { }
	
	[RPC]
	
	void SendInfoToClient(string someInfo) { }

}
