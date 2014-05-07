using UnityEngine;
using System.Collections;

public class ServerWelcome : MonoBehaviour {

	public GUISkin skin;

	/**
	 * Some constants used for network
	 * */
	private int maxClients = 16;
	private int port = 25002;
	private string serverName = "Unknown";
	public static string appName = "Mr.Piano";
	public static string appStart = "start";

	/**
	 * Some constants used for gui components
	 * */
	private Vector2 scrollPosition;
	private Rect windowRect = new Rect (10, 10, Screen.width-20, Screen.height-20);
	private string startServerLabel = "Start Server";
	private string startApplicationLabel = "Start Application";
	private string stopServerLabel = "Stop Server";
	private string title = ServerWelcome.appName+" - iPad";

	/**
	 * Some constant messages
	 * */
	private string cameraFound = "Camera Attached:";
	private string serverInit = "Initializing Server...";
	private string serverStart = "Server Started.";
	private string serverStoped = "Server Stopped.";
	private string pressStartToStart = "You can press \'Start Application\' now.";
	
	private int cameraCount = 0;
	private string infoSentToClient;

	/**
	 * System status
	 * */
	private string status = "Welcome!\n";

	private void display(string msg){
		status = msg;
	}

	/**
	 * Message sent by client
	 * */
	private string clientMsg = "";

	// Use this for initialization
	void Start () {
		serverName = SystemInfo.deviceName;
	}

	private void startServer(){
		Network.InitializeServer(maxClients,port,!Network.HavePublicAddress());
		MasterServer.RegisterHost(appName,serverName);
		display(serverInit);
		
	}
	
	private void stopServer(){
		Network.Disconnect();
		display(serverStoped);
	}

	void Update(){
		if(Input.GetMouseButtonDown(0)){
			networkView.RPC("ReceiveInfoFromServer", RPCMode.Others, "server mouse click");
		}
	}

	void OnMasterServerEvent(MasterServerEvent masterServerEvent){
		if(masterServerEvent==MasterServerEvent.RegistrationSucceeded){
			display(serverStart);
		}
	}
	
	void OnGUI () {
		if(skin!=null){
			GUI.skin = skin;
		}
		windowRect = GUI.Window (0, windowRect, WindowFunction, "");
	}

	void WindowFunction (int windowID) {
		GUILayout.Label(title);

		if(Network.isServer){
			if(cameraCount>0){
				if(GUILayout.Button(startApplicationLabel)){
					infoSentToClient = appStart;
					SendInfoToClient();
//					networkView.RPC("ReceiveInfoFromServer", RPCMode.Others, "start");
//					Application.LoadLevel("Server_main");
				}
			}
			if(GUILayout.Button(stopServerLabel)){
				stopServer();
			}
		}else{
			if(GUILayout.Button(startServerLabel)){
				startServer();
			}
		}

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		if(Network.isServer && cameraCount>0){
			GUILayout.Box(cameraCount+" cameras attached."+"\n"+pressStartToStart);
		}else{
			GUILayout.Box(status);
		}
		GUILayout.EndScrollView();
		if(GUILayout.Button("Quit")){
			Application.Quit();
		}

	}

	void OnPlayerConnected(NetworkPlayer player) {
		AskClientForInfo(player);
		cameraCount++;
	}

	void OnPlayerDisconnected(NetworkPlayer player){
		cameraCount--;
		Network.RemoveRPCs(player);
	}
	
	void AskClientForInfo(NetworkPlayer player) {
		networkView.RPC("SetPlayerInfo", player, player);
	}

	[RPC]
	
	void ReceiveInfoFromClient(string someInfo) {	
		if(someInfo!=null){
			//receive info
		}
	}

	[RPC]
	
	void SendInfoToClient() {
		Debug.Log("test");
		networkView.RPC("ReceiveInfoFromServer", RPCMode.Others, infoSentToClient);
	}
		
	[RPC]
	
	void SendInfoToServer() { }
	
	[RPC]
	
	void SetPlayerInfo(NetworkPlayer player) { }
	
	[RPC]
	
	void ReceiveInfoFromServer(string someInfo) { }
	
}
	
